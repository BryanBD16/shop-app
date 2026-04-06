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
    private const int PageSize = 12; // Assure-toi que ça match ton service

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
        context.Products.Add(new Product { Name = "Visible", IsPublished = true });
        context.Products.Add(new Product { Name = "Hidden", IsPublished = false });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductsAsync(1, "");

        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetPublishedProductsAsync_FiltersBySearch()
    {
        using var context = CreateDbContext();
        context.Products.Add(new Product { Name = "iPhone", IsPublished = true });
        context.Products.Add(new Product { Name = "Samsung", IsPublished = true });
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

        for (int i = 1; i <= 15; i++)
        {
            context.Products.Add(new Product { Name = $"Product {i}", IsPublished = true });
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

        for (int i = 1; i <= 20; i++)
        {
            context.Products.Add(new Product { Name = $"Product {i}", IsPublished = true });
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

        for (int i = 1; i <= 21; i++)
        {
            context.Products.Add(new Product { Name = $"Product {i}", IsPublished = true });
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
        context.Products.Add(new Product { Name = "iPhone", IsPublished = true });
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
        context.Products.Add(new Product { Name = "A", IsPublished = true });
        context.Products.Add(new Product { Name = "B", IsPublished = true });
        context.Products.Add(new Product { Name = "C", IsPublished = false });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductsAsync(1, "");

        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetPublishedProductsAsync_MapsToDtoCorrectly()
    {
        using var context = CreateDbContext();
        context.Products.Add(new Product
        {
            Name = "Test",
            Price = 99.99m,
            ImagePath = "image.jpg",
            IsPublished = true
        });

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductsAsync(1, "");

        var item = result.Items.First();

        Assert.Equal("Test", item.Name);
        Assert.Equal(99.99m, item.Price);
        Assert.Equal("image.jpg", item.ImagePath);
    }

    [Fact]
    public async Task GetPublishedProductsAsync_ReturnsItemsOrderedById()
    {
        using var context = CreateDbContext();

        context.Products.Add(new Product { Name = "B", IsPublished = true });
        context.Products.Add(new Product { Name = "A", IsPublished = true });

        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetPublishedProductsAsync(1, "");

        var items = result.Items.ToList();

        Assert.True(items[0].Id < items[1].Id);
    }
    #endregion

#region GetPublishedProductByIdAsync

[Fact]
public async Task GetPublishedProductByIdAsync_ReturnsProduct_WhenPublishedAndExists()
{
    using var context = CreateDbContext();

    var product = new Product
    {
        Name = "Test",
        Price = 50,
        ImagePath = "img.jpg",
        Description = "Desc",
        IsPublished = true
    };

    context.Products.Add(product);
    await context.SaveChangesAsync();

    var service = new ProductService(context);

    var result = await service.GetPublishedProductByIdAsync(product.Id);

    Assert.NotNull(result);
    Assert.Equal(product.Id, result.Id);
}

[Fact]
public async Task GetPublishedProductByIdAsync_ReturnsNull_WhenNotPublished()
{
    using var context = CreateDbContext();

    var product = new Product
    {
        Name = "Hidden",
        IsPublished = false
    };

    context.Products.Add(product);
    await context.SaveChangesAsync();

    var service = new ProductService(context);

    var result = await service.GetPublishedProductByIdAsync(product.Id);

    Assert.Null(result);
}

[Fact]
public async Task GetPublishedProductByIdAsync_ReturnsNull_WhenIdDoesNotExist()
{
    using var context = CreateDbContext();

    context.Products.Add(new Product
    {
        Name = "Test",
        IsPublished = true
    });

    await context.SaveChangesAsync();

    var service = new ProductService(context);

    var result = await service.GetPublishedProductByIdAsync(999);

    Assert.Null(result);
}

[Fact]
public async Task GetPublishedProductByIdAsync_MapsAllFieldsCorrectly()
{
    using var context = CreateDbContext();

    var product = new Product
    {
        Name = "Test",
        Price = 99.99m,
        ImagePath = "image.png",
        Description = "Full description",
        IsPublished = true
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
}

[Fact]
public async Task GetPublishedProductByIdAsync_ReturnsNull_WhenDatabaseIsEmpty()
{
    using var context = CreateDbContext();

    var service = new ProductService(context);

    var result = await service.GetPublishedProductByIdAsync(1);

    Assert.Null(result);
}

#endregion

#region GetAdminProductsAsync

[Fact]
public async Task GetAdminProductsAsync_ReturnsAllProducts_IncludingUnpublished()
{
    using var context = CreateDbContext();

    context.Products.Add(new Product { Name = "A", IsPublished = true });
    context.Products.Add(new Product { Name = "B", IsPublished = false });
    await context.SaveChangesAsync();

    var service = new ProductService(context);

    var result = await service.GetAdminProductsAsync(1, "");

    Assert.Equal(2, result.Items.Count);
}

[Fact]
public async Task GetAdminProductsAsync_FiltersBySearch()
{
    using var context = CreateDbContext();

    context.Products.Add(new Product { Name = "iPhone" });
    context.Products.Add(new Product { Name = "Samsung" });
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

    for (int i = 1; i <= 15; i++)
    {
        context.Products.Add(new Product { Name = $"Product {i}" });
    }

    await context.SaveChangesAsync();

    var service = new ProductService(context);

    var result = await service.GetAdminProductsAsync(2, "");

    Assert.Equal(3, result.Items.Count); // PageSize = 12
}

[Fact]
public async Task GetAdminProductsAsync_ReturnsCorrectTotalItems()
{
    using var context = CreateDbContext();

    for (int i = 1; i <= 20; i++)
    {
        context.Products.Add(new Product { Name = $"Product {i}" });
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

    for (int i = 1; i <= 25; i++)
    {
        context.Products.Add(new Product { Name = $"Product {i}" });
    }

    await context.SaveChangesAsync();

    var service = new ProductService(context);

    var result = await service.GetAdminProductsAsync(1, "");

    Assert.Equal(3, result.TotalPages); // 25 / 12 = 3
}

[Fact]
public async Task GetAdminProductsAsync_ReturnsEmpty_WhenNoMatch()
{
    using var context = CreateDbContext();

    context.Products.Add(new Product { Name = "iPhone" });
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

    var product = new Product
    {
        Name = "Test",
        Price = 99.99m,
        ImagePath = "image.png",
        StockQuantity = 5,
        IsPublished = true
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
}

[Fact]
public async Task GetAdminProductsAsync_ReturnsItemsOrderedById()
{
    using var context = CreateDbContext();

    context.Products.Add(new Product { Name = "B" });
    context.Products.Add(new Product { Name = "A" });

    await context.SaveChangesAsync();

    var service = new ProductService(context);

    var result = await service.GetAdminProductsAsync(1, "");

    var items = result.Items.ToList();

    Assert.True(items[0].Id < items[1].Id);
}

#endregion

#region GetAdminProductByIdAsync

[Fact]
public async Task GetAdminProductByIdAsync_ReturnsProduct_WhenExists()
{
    using var context = CreateDbContext();

    var product = new Product
    {
        Name = "Test",
        Price = 50,
        ImagePath = "img.jpg",
        Description = "Desc",
        StockQuantity = 10,
        IsPublished = true
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

    var product = new Product
    {
        Name = "Hidden",
        IsPublished = false
    };

    context.Products.Add(product);
    await context.SaveChangesAsync();

    var service = new ProductService(context);

    var result = await service.GetAdminProductByIdAsync(product.Id);

    Assert.NotNull(result);
    Assert.Equal(product.Id, result.Id);
}

[Fact]
public async Task GetAdminProductByIdAsync_ReturnsNull_WhenIdDoesNotExist()
{
    using var context = CreateDbContext();

    context.Products.Add(new Product
    {
        Name = "Test",
        IsPublished = true
    });

    await context.SaveChangesAsync();

    var service = new ProductService(context);

    var result = await service.GetAdminProductByIdAsync(999);

    Assert.Null(result);
}

[Fact]
public async Task GetAdminProductByIdAsync_MapsAllFieldsCorrectly()
{
    using var context = CreateDbContext();

    var product = new Product
    {
        Name = "Test",
        Price = 99.99m,
        ImagePath = "image.png",
        Description = "Full description",
        StockQuantity = 5,
        IsPublished = true
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
}

[Fact]
public async Task GetAdminProductByIdAsync_ReturnsNull_WhenDatabaseIsEmpty()
{
    using var context = CreateDbContext();

    var service = new ProductService(context);

    var result = await service.GetAdminProductByIdAsync(1);

    Assert.Null(result);
}

#endregion

#region CreateProductAsync

[Fact]
public async Task CreateProductAsync_CreatesProduct_AndReturnsId()
{
    using var context = CreateDbContext();

    var dto = new AdminProductCreateDto
    {
        Name = "Test",
        Price = 100,
        ImagePath = "/images/test.jpg",
        Description = "Desc",
        StockQuantity = 5,
        IsPublished = true
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
}

[Fact]
public async Task CreateProductAsync_ThrowsFileNotFoundException_WhenImageDoesNotExist()
{
    using var context = CreateDbContext();

    var dto = new AdminProductCreateDto
    {
        Name = "Test",
        Price = 100,
        ImagePath = "/images/missing.jpg",
        Description = "Desc",
        StockQuantity = 5,
        IsPublished = true
    };

    var service = new ProductService(context);

    await Assert.ThrowsAsync<FileNotFoundException>(() =>
        service.CreateProductAsync(dto)
    );
}

[Fact]
public async Task CreateProductAsync_SavesProductInDatabase()
{
    using var context = CreateDbContext();

    var dto = new AdminProductCreateDto
    {
        Name = "DB Test",
        Price = 50,
        ImagePath = "/images/db.jpg",
        Description = "Desc",
        StockQuantity = 2,
        IsPublished = false
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

    var dto = new AdminProductCreateDto
    {
        Name = "Id Test",
        Price = 10,
        ImagePath = "/images/id.jpg",
        Description = "Desc",
        StockQuantity = 1,
        IsPublished = true
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

    var product = new Product
    {
        Name = "Old",
        Price = 10,
        ImagePath = "old.jpg",
        Description = "Old desc",
        StockQuantity = 1,
        IsPublished = false
    };

    context.Products.Add(product);
    await context.SaveChangesAsync();

    var dto = new AdminProductUpdateDto
    {
        Name = "New",
        Price = 99,
        ImagePath = "new.jpg",
        Description = "New desc",
        StockQuantity = 5,
        IsPublished = true
    };

    var service = new ProductService(context);

    var result = await service.UpdateProductAsync(product.Id, dto);

    var updated = await context.Products.FindAsync(product.Id);

    Assert.True(result);
    Assert.Equal("New", updated!.Name);
    Assert.Equal(99, updated.Price);
    Assert.Equal("new.jpg", updated.ImagePath);
    Assert.Equal("New desc", updated.Description);
    Assert.Equal(5, updated.StockQuantity);
    Assert.True(updated.IsPublished);
}

[Fact]
public async Task UpdateProductAsync_ReturnsFalse_WhenProductDoesNotExist()
{
    using var context = CreateDbContext();

    var dto = new AdminProductUpdateDto
    {
        Name = "Test",
        Price = 10,
        ImagePath = "img.jpg",
        Description = "Desc",
        StockQuantity = 1,
        IsPublished = true
    };

    var service = new ProductService(context);

    var result = await service.UpdateProductAsync(999, dto);

    Assert.False(result);
}

[Fact]
public async Task UpdateProductAsync_DoesNotChangeOtherProducts()
{
    using var context = CreateDbContext();

    var product1 = new Product { Name = "A", Price = 10 };
    var product2 = new Product { Name = "B", Price = 20 };

    context.Products.AddRange(product1, product2);
    await context.SaveChangesAsync();

    var dto = new AdminProductUpdateDto
    {
        Name = "Updated",
        Price = 999,
        ImagePath = "img.jpg",
        Description = "Desc",
        StockQuantity = 5,
        IsPublished = true
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

    var product = new Product
    {
        Name = "Before",
        Price = 10
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
        IsPublished = false
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

#region DeleteProductAsync

[Fact]
public async Task DeleteProductAsync_ShouldReturnTrue_WhenProductExists()
{
    using var context = CreateDbContext();
    var service = new ProductService(context);

    // Arrange
    var product = new Product
    {
        Id = 1,
        Name = "Test Product"
    };

    context.Products.Add(product);
    await context.SaveChangesAsync();

    // Act
    var result = await service.DeleteProductAsync(1);

    // Assert
    Assert.True(result);
    Assert.Empty(context.Products);
}

[Fact]
public async Task DeleteProductAsync_ShouldReturnFalse_WhenProductDoesNotExist()
{
    using var context = CreateDbContext();
    var service = new ProductService(context);

    // Act
    var result = await service.DeleteProductAsync(999);

    // Assert
    Assert.False(result);
}

[Fact]
public async Task DeleteProductAsync_ShouldOnlyRemoveCorrectProduct_WhenMultipleExist()
{
    using var context = CreateDbContext();
    var service = new ProductService(context);

    // Arrange
    var product1 = new Product { Id = 1, Name = "P1" };
    var product2 = new Product { Id = 2, Name = "P2" };
    var product3 = new Product { Id = 3, Name = "P3" };

    context.Products.AddRange(product1, product2, product3);
    await context.SaveChangesAsync();

    // Act
    var result = await service.DeleteProductAsync(2);

    // Assert
    Assert.True(result);
    Assert.Equal(2, context.Products.Count());
    Assert.DoesNotContain(context.Products, p => p.Id == 2);
    Assert.Contains(context.Products, p => p.Id == 1);
    Assert.Contains(context.Products, p => p.Id == 3);
}

[Fact]
public async Task DeleteProductAsync_ShouldPersistChangesAfterDeletion()
{
    using var context = CreateDbContext();
    var service = new ProductService(context);

    // Arrange
    var product = new Product { Id = 1, Name = "Test" };
    context.Products.Add(product);
    await context.SaveChangesAsync();

    // Act
    await service.DeleteProductAsync(1);

    var exists = await context.Products.FindAsync(1);

    // Assert
    Assert.Null(exists);
}

#endregion

#region GetProductImages Tests

[Fact]
public void GetProductImages_ShouldReturnEmptyList_WhenDirectoryDoesNotExist()
{
    using var context = CreateDbContext();
    var service = new ProductService(context);

    // Arrange
    var expectedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");

    // Ensure directory does NOT exist
    if (Directory.Exists(expectedPath))
        Directory.Delete(expectedPath, true);

    // Act
    var result = service.GetProductImages();

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);
}

[Fact]
public void GetProductImages_ShouldReturnEmptyList_WhenDirectoryIsEmpty()
{
    using var context = CreateDbContext();
    var service = new ProductService(context);

    // Arrange
    var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
    Directory.CreateDirectory(imageDir);

    // Clean directory
    foreach (var file in Directory.GetFiles(imageDir))
    {
        File.Delete(file);
    }

    // Act
    var result = service.GetProductImages();

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);
}

[Fact]
public void GetProductImages_ShouldReturnImagePaths_WhenFilesExist()
{
    using var context = CreateDbContext();
    var service = new ProductService(context);

    // Arrange
    var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
    Directory.CreateDirectory(imageDir);

    var file1 = Path.Combine(imageDir, "img1.jpg");
    var file2 = Path.Combine(imageDir, "img2.png");

    File.WriteAllText(file1, "fake");
    File.WriteAllText(file2, "fake");

    // Act
    var result = service.GetProductImages();

    // Assert
    Assert.Contains("/images/products/img1.jpg", result);
    Assert.Contains("/images/products/img2.png", result);
    Assert.Equal(2, result.Count);
}

#endregion

}