using BackendApi.Data;
using BackendApi.Models;
using BackendApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using BackendApi.Dtos.Admin;

namespace BackendApi.Tests.Unit.Services;

public class ProductServiceTests
{
    private const int PageSize = 12;

    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

#region GetPublishedProductsAsync

    [Fact]
    public async Task GetPublishedProductsAsync_ReturnsOnlyPublishedProducts()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Products.Add(new Product { Name = "Visible", IsPublished = true, CategoryId = category.Id });
        context.Products.Add(new Product { Name = "Hidden", IsPublished = false, CategoryId = category.Id });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductsAsync(1, "");

        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetPublishedProductsAsync_FiltersBySearch()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Products.Add(new Product { Name = "iPhone", IsPublished = true, CategoryId = category.Id });
        context.Products.Add(new Product { Name = "Samsung", IsPublished = true, CategoryId = category.Id });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductsAsync(1, "Phone");

        Assert.Single(result.Items);
        Assert.Contains(result.Items, p => p.Name == "iPhone");
    }

    [Fact]
    public async Task GetPublishedProductsAsync_ReturnsCorrectPageItems()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        for (int i = 1; i <= 15; i++)
        {
            context.Products.Add(new Product { Name = $"Product {i}", IsPublished = true, CategoryId = category.Id });
        }

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductsAsync(2, "");

        Assert.Equal(3, result.Items.Count);
    }

    [Fact]
    public async Task GetPublishedProductsAsync_ReturnsCorrectTotalItems()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        for (int i = 1; i <= 20; i++)
        {
            context.Products.Add(new Product { Name = $"Product {i}", IsPublished = true, CategoryId = category.Id });
        }

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductsAsync(1, "");

        Assert.Equal(PageSize, result.Items.Count);
        Assert.Equal(20, result.TotalItems);
    }

    [Fact]
    public async Task GetPublishedProductsAsync_CalculatesTotalPagesCorrectly()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        for (int i = 1; i <= 21; i++)
        {
            context.Products.Add(new Product { Name = $"Product {i}", IsPublished = true, CategoryId = category.Id });
        }

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductsAsync(1, "");

        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetPublishedProductsAsync_ReturnsEmptyWhenNoMatch()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Products.Add(new Product { Name = "iPhone", IsPublished = true, CategoryId = category.Id });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductsAsync(1, "XYZ");

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalItems);
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public async Task GetPublishedProductsAsync_ReturnsAllPublishedWhenSearchEmpty()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Products.Add(new Product { Name = "A", IsPublished = true, CategoryId = category.Id });
        context.Products.Add(new Product { Name = "B", IsPublished = true, CategoryId = category.Id });
        context.Products.Add(new Product { Name = "C", IsPublished = false, CategoryId = category.Id });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductsAsync(1, "");

        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetPublishedProductsAsync_MapsToDtoCorrectly()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Products.Add(new Product
        {
            Name = "Test",
            Price = 99.99m,
            ImagePath = "image.jpg",
            IsPublished = true,
            CategoryId = category.Id
        });

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductsAsync(1, "");

        var item = result.Items.First();

        Assert.Equal("Test", item.Name);
        Assert.Equal(99.99m, item.Price);
        Assert.Equal("image.jpg", item.ImagePath);
        Assert.Equal(category.Id, item.CategoryId);
    }

    [Fact]
    public async Task GetPublishedProductsAsync_ReturnsItemsOrderedByName()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Products.Add(new Product { Name = "B", IsPublished = true, CategoryId = category.Id });
        context.Products.Add(new Product { Name = "A", IsPublished = true, CategoryId = category.Id });

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductsAsync(1, "");

        var items = result.Items.ToList();

        Assert.Equal("A", items[0].Name);
        Assert.Equal("B", items[1].Name);
    }

    [Fact]
    public async Task GetPublishedProductsAsync_FiltersByCategory()
    {
        using var context = CreateDbContext();
        var electronics = new Category { Name = "Electronics" };
        var books = new Category { Name = "Books" };
        context.Categories.AddRange(electronics, books);
        await context.SaveChangesAsync();

        context.Products.Add(new Product { Name = "iPhone", IsPublished = true, CategoryId = electronics.Id });
        context.Products.Add(new Product { Name = "Samsung", IsPublished = true, CategoryId = electronics.Id });
        context.Products.Add(new Product { Name = "Novel", IsPublished = true, CategoryId = books.Id });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductsAsync(1, "", electronics.Id);

        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, item => Assert.Equal(electronics.Id, item.CategoryId));
    }

#endregion

#region GetPublishedProductByIdAsync

    [Fact]
    public async Task GetPublishedProductByIdAsync_ReturnsProduct_WhenPublishedAndExists()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Test",
            Price = 50,
            ImagePath = "img.jpg",
            Description = "Desc",
            IsPublished = true,
            CategoryId = category.Id
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(category.Id, result.CategoryId);
    }

    [Fact]
    public async Task GetPublishedProductByIdAsync_ThrowsKeyNotFoundException_WhenNotPublished()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Hidden",
            IsPublished = false,
            CategoryId = category.Id
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetPublishedProductByIdAsync(product.Id));
    }

    [Fact]
    public async Task GetPublishedProductByIdAsync_ThrowsKeyNotFoundException_WhenIdDoesNotExist()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Products.Add(new Product
        {
            Name = "Test",
            IsPublished = true,
            CategoryId = category.Id
        });

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetPublishedProductByIdAsync(999));
    }

    [Fact]
    public async Task GetPublishedProductByIdAsync_MapsAllFieldsCorrectly()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Test",
            Price = 99.99m,
            ImagePath = "image.png",
            Description = "Full description",
            IsPublished = true,
            CategoryId = category.Id
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
        Assert.Equal(99.99m, result.Price);
        Assert.Equal("image.png", result.ImagePath);
        Assert.Equal("Full description", result.Description);
        Assert.Equal(category.Id, result.CategoryId);
    }

    [Fact]
    public async Task GetPublishedProductByIdAsync_ThrowsKeyNotFoundException_WhenDatabaseIsEmpty()
    {
        using var context = CreateDbContext();

        var service = new ProductService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetPublishedProductByIdAsync(1));
    }

    [Fact]
    public async Task GetPublishedProductByIdAsync_IncludesCategoryId()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Books" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Test Book",
            Price = 25.99m,
            IsPublished = true,
            CategoryId = category.Id
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal(category.Id, result.CategoryId);
    }

#endregion

#region GetAdminProductsAsync

    [Fact]
    public async Task GetAdminProductsAsync_ReturnsAllProducts_IncludingUnpublished()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Products.Add(new Product { Name = "A", IsPublished = true, CategoryId = category.Id });
        context.Products.Add(new Product { Name = "B", IsPublished = false, CategoryId = category.Id });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetAdminProductsAsync(1, "");

        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetAdminProductsAsync_FiltersBySearch()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Products.Add(new Product { Name = "iPhone", CategoryId = category.Id });
        context.Products.Add(new Product { Name = "Samsung", CategoryId = category.Id });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetAdminProductsAsync(1, "Phone");

        Assert.Single(result.Items);
        Assert.Contains(result.Items, p => p.Name == "iPhone");
    }

    [Fact]
    public async Task GetAdminProductsAsync_ReturnsCorrectPagination()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        for (int i = 1; i <= 15; i++)
        {
            context.Products.Add(new Product { Name = $"Product {i}", CategoryId = category.Id });
        }

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetAdminProductsAsync(2, "");

        Assert.Equal(3, result.Items.Count);
    }

    [Fact]
    public async Task GetAdminProductsAsync_ReturnsCorrectTotalItems()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        for (int i = 1; i <= 20; i++)
        {
            context.Products.Add(new Product { Name = $"Product {i}", CategoryId = category.Id });
        }

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetAdminProductsAsync(1, "");

        Assert.Equal(20, result.TotalItems);
    }

    [Fact]
    public async Task GetAdminProductsAsync_CalculatesTotalPagesCorrectly()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        for (int i = 1; i <= 25; i++)
        {
            context.Products.Add(new Product { Name = $"Product {i}", CategoryId = category.Id });
        }

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetAdminProductsAsync(1, "");

        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public async Task GetAdminProductsAsync_ReturnsEmpty_WhenNoMatch()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Products.Add(new Product { Name = "iPhone", CategoryId = category.Id });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetAdminProductsAsync(1, "XYZ");

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalItems);
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public async Task GetAdminProductsAsync_MapsAllFieldsCorrectly()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Test",
            Price = 99.99m,
            ImagePath = "image.png",
            StockQuantity = 5,
            IsPublished = true,
            CategoryId = category.Id
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetAdminProductsAsync(1, "");

        var item = result.Items.First();

        Assert.Equal("Test", item.Name);
        Assert.Equal(99.99m, item.Price);
        Assert.Equal("image.png", item.ImagePath);
        Assert.Equal(5, item.StockQuantity);
        Assert.Equal(true, item.IsPublished);
        Assert.Equal(category.Id, item.CategoryId);
    }

    [Fact]
    public async Task GetAdminProductsAsync_ReturnsItemsOrderedByName()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Products.Add(new Product { Name = "B", CategoryId = category.Id });
        context.Products.Add(new Product { Name = "A", CategoryId = category.Id });

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetAdminProductsAsync(1, "");

        var items = result.Items.ToList();

        Assert.Equal("A", items[0].Name);
        Assert.Equal("B", items[1].Name);
    }

    [Fact]
    public async Task GetAdminProductsAsync_FiltersByCategory()
    {
        using var context = CreateDbContext();
        var electronics = new Category { Name = "Electronics" };
        var books = new Category { Name = "Books" };
        context.Categories.AddRange(electronics, books);
        await context.SaveChangesAsync();

        context.Products.Add(new Product { Name = "iPhone", CategoryId = electronics.Id });
        context.Products.Add(new Product { Name = "Samsung", CategoryId = electronics.Id });
        context.Products.Add(new Product { Name = "Novel", CategoryId = books.Id });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetAdminProductsAsync(1, "", electronics.Id);

        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, item => Assert.Equal(electronics.Id, item.CategoryId));
    }

#endregion

#region GetAdminProductByIdAsync

    [Fact]
    public async Task GetAdminProductByIdAsync_ReturnsProduct_WhenExists()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Test",
            Price = 50,
            ImagePath = "img.jpg",
            Description = "Desc",
            StockQuantity = 10,
            IsPublished = true,
            CategoryId = category.Id
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetAdminProductByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
    }

    [Fact]
    public async Task GetAdminProductByIdAsync_ReturnsProduct_EvenIfNotPublished()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Hidden",
            IsPublished = false,
            CategoryId = category.Id
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetAdminProductByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
    }

    [Fact]
    public async Task GetAdminProductByIdAsync_ThrowsKeyNotFoundException_WhenIdDoesNotExist()
    {
        using var context = CreateDbContext();

        context.Products.Add(new Product
        {
            Name = "Test",
            IsPublished = true
        });

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetAdminProductByIdAsync(999));
    }

    [Fact]
    public async Task GetAdminProductByIdAsync_MapsAllFieldsCorrectly()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Test",
            Price = 99.99m,
            ImagePath = "image.png",
            Description = "Full description",
            StockQuantity = 5,
            IsPublished = true,
            CategoryId = category.Id
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetAdminProductByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
        Assert.Equal(99.99m, result.Price);
        Assert.Equal("image.png", result.ImagePath);
        Assert.Equal("Full description", result.Description);
        Assert.Equal(5, result.StockQuantity);
        Assert.Equal(true, result.IsPublished);
        Assert.Equal(category.Id, result.CategoryId);
    }

    [Fact]
    public async Task GetAdminProductByIdAsync_ThrowsKeyNotFoundException_WhenDatabaseIsEmpty()
    {
        using var context = CreateDbContext();

        var service = new ProductService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetAdminProductByIdAsync(1));
    }

#endregion

#region CreateProductAsync

    [Fact]
    public async Task CreateProductAsync_CreatesProduct_AndReturnsId()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var dto = new AdminProductCreateDto
        {
            Name = "Test",
            Price = 100,
            ImagePath = "/images/test.jpg",
            Description = "Desc",
            StockQuantity = 5,
            IsPublished = true,
            CategoryId = category.Id
        };

        var imageFullPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            dto.ImagePath.TrimStart('/')
        );

        Directory.CreateDirectory(Path.GetDirectoryName(imageFullPath)!);
        await File.WriteAllTextAsync(imageFullPath, "fake image");

        var service = new ProductService(context);

        var resultId = await service.CreateProductAsync(dto);

        var product = await context.Products.FindAsync(resultId);

        Assert.NotNull(product);
        Assert.Equal(dto.Name, product!.Name);
        Assert.Equal(dto.Price, product.Price);
        Assert.Equal(dto.ImagePath, product.ImagePath);
        Assert.Equal(dto.Description, product.Description);
        Assert.Equal(dto.StockQuantity, product.StockQuantity);
        Assert.Equal(dto.IsPublished, product.IsPublished);
        Assert.Equal(dto.CategoryId, product.CategoryId);
    }

    [Fact]
    public async Task CreateProductAsync_ThrowsInvalidOperationException_WhenCategoryDoesNotExist()
    {
        using var context = CreateDbContext();

        var dto = new AdminProductCreateDto
        {
            Name = "Test",
            Price = 100,
            ImagePath = "/images/test.jpg",
            Description = "Desc",
            StockQuantity = 5,
            IsPublished = true,
            CategoryId = 999
        };

        var imageFullPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            dto.ImagePath.TrimStart('/')
        );

        Directory.CreateDirectory(Path.GetDirectoryName(imageFullPath)!);
        await File.WriteAllTextAsync(imageFullPath, "fake image");

        var service = new ProductService(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateProductAsync(dto)
        );
    }

    [Fact]
    public async Task CreateProductAsync_ThrowsInvalidOperationException_WhenImageDoesNotExist()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var dto = new AdminProductCreateDto
        {
            Name = "Test",
            Price = 100,
            ImagePath = "/images/missing.jpg",
            Description = "Desc",
            StockQuantity = 5,
            IsPublished = true,
            CategoryId = category.Id
        };

        var service = new ProductService(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateProductAsync(dto)
        );
    }

    [Fact]
    public async Task CreateProductAsync_SavesProductInDatabase()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var dto = new AdminProductCreateDto
        {
            Name = "DB Test",
            Price = 50,
            ImagePath = "/images/db.jpg",
            Description = "Desc",
            StockQuantity = 2,
            IsPublished = false,
            CategoryId = category.Id
        };

        var imageFullPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            dto.ImagePath.TrimStart('/')
        );

        Directory.CreateDirectory(Path.GetDirectoryName(imageFullPath)!);
        await File.WriteAllTextAsync(imageFullPath, "fake image");

        var service = new ProductService(context);

        var id = await service.CreateProductAsync(dto);

        var exists = await context.Products.AnyAsync(p => p.Id == id);

        Assert.True(exists);
    }

    [Fact]
    public async Task CreateProductAsync_ReturnsValidId()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var dto = new AdminProductCreateDto
        {
            Name = "Id Test",
            Price = 10,
            ImagePath = "/images/id.jpg",
            Description = "Desc",
            StockQuantity = 1,
            IsPublished = true,
            CategoryId = category.Id
        };

        var imageFullPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            dto.ImagePath.TrimStart('/')
        );

        Directory.CreateDirectory(Path.GetDirectoryName(imageFullPath)!);
        await File.WriteAllTextAsync(imageFullPath, "fake image");

        var service = new ProductService(context);

        var id = await service.CreateProductAsync(dto);

        Assert.True(id > 0);
    }

#endregion

#region UpdateProductAsync

    [Fact]
    public async Task UpdateProductAsync_ReturnsTrue_AndUpdatesProduct_WhenExists()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Old",
            Price = 10,
            ImagePath = "old.jpg",
            Description = "Old desc",
            StockQuantity = 1,
            IsPublished = false,
            CategoryId = category.Id
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var newCategory = new Category { Name = "Books" };
        context.Categories.Add(newCategory);
        await context.SaveChangesAsync();

        var dto = new AdminProductUpdateDto
        {
            Name = "New",
            Price = 99,
            ImagePath = "new.jpg",
            Description = "New desc",
            StockQuantity = 5,
            IsPublished = true,
            CategoryId = newCategory.Id
        };

        var service = new ProductService(context);

        await service.UpdateProductAsync(product.Id, dto);

        var updated = await context.Products.FindAsync(product.Id);

        Assert.Equal("New", updated!.Name);
        Assert.Equal(99, updated.Price);
        Assert.Equal("new.jpg", updated.ImagePath);
        Assert.Equal("New desc", updated.Description);
        Assert.Equal(5, updated.StockQuantity);
        Assert.True(updated.IsPublished);
        Assert.Equal(newCategory.Id, updated.CategoryId);
    }

    [Fact]
    public async Task UpdateProductAsync_ThrowsKeyNotFoundException_WhenProductDoesNotExist()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var dto = new AdminProductUpdateDto
        {
            Name = "Test",
            Price = 10,
            ImagePath = "img.jpg",
            Description = "Desc",
            StockQuantity = 1,
            IsPublished = true,
            CategoryId = category.Id
        };

        var service = new ProductService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateProductAsync(999, dto));
    }

    [Fact]
    public async Task UpdateProductAsync_ThrowsInvalidOperationException_WhenCategoryDoesNotExist()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product { Name = "Test", CategoryId = category.Id };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var dto = new AdminProductUpdateDto
        {
            Name = "Test",
            Price = 10,
            ImagePath = "img.jpg",
            Description = "Desc",
            StockQuantity = 1,
            IsPublished = true,
            CategoryId = 999
        };

        var service = new ProductService(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateProductAsync(product.Id, dto)
        );
    }

    [Fact]
    public async Task UpdateProductAsync_DoesNotChangeOtherProducts()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product1 = new Product { Name = "A", Price = 10, CategoryId = category.Id };
        var product2 = new Product { Name = "B", Price = 20, CategoryId = category.Id };

        context.Products.AddRange(product1, product2);
        await context.SaveChangesAsync();

        var dto = new AdminProductUpdateDto
        {
            Name = "Updated",
            Price = 999,
            ImagePath = "img.jpg",
            Description = "Desc",
            StockQuantity = 5,
            IsPublished = true,
            CategoryId = category.Id
        };

        var service = new ProductService(context);

        await service.UpdateProductAsync(product1.Id, dto);

        var unchanged = await context.Products.FindAsync(product2.Id);

        Assert.Equal("B", unchanged!.Name);
        Assert.Equal(20, unchanged.Price);
    }

    [Fact]
    public async Task UpdateProductAsync_PersistsChangesInDatabase()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Before",
            Price = 10,
            CategoryId = category.Id
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var dto = new AdminProductUpdateDto
        {
            Name = "After",
            Price = 50,
            ImagePath = "img.jpg",
            Description = "Desc",
            StockQuantity = 2,
            IsPublished = false,
            CategoryId = category.Id
        };

        var service = new ProductService(context);

        await service.UpdateProductAsync(product.Id, dto);

        var exists = await context.Products.AnyAsync(p =>
            p.Id == product.Id &&
            p.Name == "After"
        );

        Assert.True(exists);
    }

#endregion

#region GetProductImages Tests

    [Fact]
    public void GetProductImages_ShouldReturnEmptyList_WhenDirectoryDoesNotExist()
    {
        using var context = CreateDbContext();
        var service = new ProductService(context);

        var expectedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");

        if (Directory.Exists(expectedPath))
            Directory.Delete(expectedPath, true);

        var result = service.GetProductImages();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetProductImages_ShouldReturnEmptyList_WhenDirectoryIsEmpty()
    {
        using var context = CreateDbContext();
        var service = new ProductService(context);

        var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
        Directory.CreateDirectory(imageDir);

        foreach (var file in Directory.GetFiles(imageDir))
        {
            File.Delete(file);
        }

        var result = service.GetProductImages();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetProductImages_ShouldReturnImagePaths_WhenFilesExist()
    {
        using var context = CreateDbContext();
        var service = new ProductService(context);

        var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
        Directory.CreateDirectory(imageDir);

        var file1 = Path.Combine(imageDir, "img1.jpg");
        var file2 = Path.Combine(imageDir, "img2.png");

        File.WriteAllText(file1, "fake");
        File.WriteAllText(file2, "fake");

        var result = service.GetProductImages();

        Assert.Contains("/images/products/img1.jpg", result);
        Assert.Contains("/images/products/img2.png", result);
        Assert.Equal(2, result.Count);
    }

#endregion

}
