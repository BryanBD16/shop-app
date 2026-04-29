using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Dtos.Admin;
using BackendApi.Models;
using BackendApi.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BackendApi.Tests.Unit.Services;

public class DiscountServiceTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

#region CreateDiscountAsync

    [Fact]
    public async Task CreateDiscountAsync_CreatesDiscountAndReturnsId_WithProduct()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        
        var product = new Product { Name = "Laptop", CategoryId = category.Id };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var futureDate = DateTime.UtcNow.AddDays(1);
        var dto = new AdminDiscountCreateDto
        {
            Title = "Summer Sale",
            Percentage = 20,
            StartDate = futureDate,
            EndDate = futureDate.AddDays(7),
            ProductId = product.Id,
            CategoryId = null
        };

        var discountId = await service.CreateDiscountAsync(dto);

        Assert.True(discountId > 0);
        var createdDiscount = await context.Discounts.FindAsync(discountId);
        Assert.NotNull(createdDiscount);
        Assert.Equal("Summer Sale", createdDiscount!.Title);
        Assert.Equal(20, createdDiscount.Percentage);
        Assert.Equal(product.Id, createdDiscount.ProductId);
        Assert.Null(createdDiscount.CategoryId);
    }

    [Fact]
    public async Task CreateDiscountAsync_CreatesDiscountAndReturnsId_WithCategory()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Books" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var futureDate = DateTime.UtcNow.AddDays(1);
        var dto = new AdminDiscountCreateDto
        {
            Title = "Book Discount",
            Percentage = 15,
            StartDate = futureDate,
            EndDate = null,
            ProductId = null,
            CategoryId = category.Id
        };

        var discountId = await service.CreateDiscountAsync(dto);

        Assert.True(discountId > 0);
        var createdDiscount = await context.Discounts.FindAsync(discountId);
        Assert.NotNull(createdDiscount);
        Assert.Equal(category.Id, createdDiscount!.CategoryId);
        Assert.Null(createdDiscount.ProductId);
    }

    [Fact]
    public async Task CreateDiscountAsync_AcceptsNameAtMaxLengthLimit()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var futureDate = DateTime.UtcNow.AddDays(1);
        var dto = new AdminDiscountCreateDto
        {
            Title = new string('A', 255),
            Percentage = 10,
            StartDate = futureDate,
            EndDate = null,
            ProductId = null,
            CategoryId = category.Id
        };

        var discountId = await service.CreateDiscountAsync(dto);

        var createdDiscount = await context.Discounts.FindAsync(discountId);
        Assert.NotNull(createdDiscount);
        Assert.Equal(255, createdDiscount!.Title.Length);
    }

    [Fact]
    public async Task CreateDiscountAsync_ThrowsInvalidOperationException_WhenBothProductAndCategoryProvided()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        
        var product = new Product { Name = "Laptop", CategoryId = category.Id };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var futureDate = DateTime.UtcNow.AddDays(1);
        var dto = new AdminDiscountCreateDto
        {
            Title = "Invalid Discount",
            Percentage = 20,
            StartDate = futureDate,
            EndDate = null,
            ProductId = product.Id,
            CategoryId = category.Id
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateDiscountAsync(dto));
    }

    [Fact]
    public async Task CreateDiscountAsync_ThrowsInvalidOperationException_WhenNeitherProductNorCategoryProvided()
    {
        using var context = CreateDbContext();

        var service = new DiscountService(context);
        var futureDate = DateTime.UtcNow.AddDays(1);
        var dto = new AdminDiscountCreateDto
        {
            Title = "Invalid Discount",
            Percentage = 20,
            StartDate = futureDate,
            EndDate = null,
            ProductId = null,
            CategoryId = null
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateDiscountAsync(dto));
    }

    [Fact]
    public async Task CreateDiscountAsync_ThrowsInvalidOperationException_WhenStartDateIsInPast()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var dto = new AdminDiscountCreateDto
        {
            Title = "Past Discount",
            Percentage = 20,
            StartDate = pastDate,
            EndDate = null,
            ProductId = null,
            CategoryId = category.Id
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateDiscountAsync(dto));
    }

    [Fact]
    public async Task CreateDiscountAsync_ThrowsInvalidOperationException_WhenEndDateIsInPast()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var futureDate = DateTime.UtcNow.AddDays(1);
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var dto = new AdminDiscountCreateDto
        {
            Title = "Invalid End Date",
            Percentage = 20,
            StartDate = futureDate,
            EndDate = pastDate,
            ProductId = null,
            CategoryId = category.Id
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateDiscountAsync(dto));
    }

    [Fact]
    public async Task CreateDiscountAsync_ThrowsInvalidOperationException_WhenOverlappingProductDiscount()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Laptop", CategoryId = 1 };
        context.Categories.Add(category);
        context.Products.Add(product);
        
        var existingDiscount = new Discount
        {
            Title = "Existing Discount",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5),
            ProductId = product.Id
        };
        context.Discounts.Add(existingDiscount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var overlappingDto = new AdminDiscountCreateDto
        {
            Title = "Overlapping Discount",
            Percentage = 15,
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(7),
            ProductId = product.Id,
            CategoryId = null
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateDiscountAsync(overlappingDto));
    }

    [Fact]
    public async Task CreateDiscountAsync_ThrowsInvalidOperationException_WhenOverlappingCategoryDiscount()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        
        var existingDiscount = new Discount
        {
            Title = "Category Discount",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(10),
            CategoryId = category.Id
        };
        context.Discounts.Add(existingDiscount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var overlappingDto = new AdminDiscountCreateDto
        {
            Title = "Overlapping Category Discount",
            Percentage = 20,
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(15),
            ProductId = null,
            CategoryId = category.Id
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateDiscountAsync(overlappingDto));
    }

    [Fact]
    public async Task CreateDiscountAsync_AllowsNonOverlappingProductDiscounts()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Laptop", CategoryId = 1 };
        context.Categories.Add(category);
        context.Products.Add(product);
        
        var firstDiscount = new Discount
        {
            Title = "First Discount",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5),
            ProductId = product.Id
        };
        context.Discounts.Add(firstDiscount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var nonOverlappingDto = new AdminDiscountCreateDto
        {
            Title = "Second Discount",
            Percentage = 15,
            StartDate = DateTime.UtcNow.AddDays(6),
            EndDate = DateTime.UtcNow.AddDays(10),
            ProductId = product.Id,
            CategoryId = null
        };

        var discountId = await service.CreateDiscountAsync(nonOverlappingDto);
        Assert.True(discountId > 0);
    }

    [Fact]
    public async Task CreateDiscountAsync_ThrowsInvalidOperationException_WhenExistingDiscountWithoutEndDate()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        
        var existingDiscount = new Discount
        {
            Title = "Ongoing Discount",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = null,
            CategoryId = category.Id
        };
        context.Discounts.Add(existingDiscount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var newDto = new AdminDiscountCreateDto
        {
            Title = "New Discount",
            Percentage = 20,
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = null,
            ProductId = null,
            CategoryId = category.Id
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateDiscountAsync(newDto));
    }

    [Fact]
    public async Task CreateDiscountAsync_AllowsDiscountAfterExistingEndDate()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        
        var product = new Product { Name = "Phone", CategoryId = category.Id };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        
        var existingDiscount = new Discount
        {
            Title = "First Sale",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5),
            ProductId = product.Id
        };
        context.Discounts.Add(existingDiscount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var newDto = new AdminDiscountCreateDto
        {
            Title = "Second Sale",
            Percentage = 15,
            StartDate = DateTime.UtcNow.AddDays(6),
            EndDate = DateTime.UtcNow.AddDays(10),
            ProductId = product.Id,
            CategoryId = null
        };

        var discountId = await service.CreateDiscountAsync(newDto);
        Assert.True(discountId > 0);
    }

    [Fact]
    public async Task CreateDiscountAsync_AllowsProductAndCategoryDiscountsSamePeriod()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Laptop", CategoryId = 1 };
        context.Categories.Add(category);
        context.Products.Add(product);
        
        var categoryDiscount = new Discount
        {
            Title = "Category Discount",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(10),
            CategoryId = category.Id
        };
        context.Discounts.Add(categoryDiscount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var productDto = new AdminDiscountCreateDto
        {
            Title = "Product Discount",
            Percentage = 20,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(10),
            ProductId = product.Id,
            CategoryId = null
        };

        var discountId = await service.CreateDiscountAsync(productDto);
        Assert.True(discountId > 0);
    }

    [Fact]
    public async Task CreateDiscountAsync_AllowsDiscountWithoutEndDate()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var futureDate = DateTime.UtcNow.AddDays(1);
        var dto = new AdminDiscountCreateDto
        {
            Title = "Ongoing Discount",
            Percentage = 25,
            StartDate = futureDate,
            EndDate = null,
            ProductId = null,
            CategoryId = category.Id
        };

        var discountId = await service.CreateDiscountAsync(dto);
        Assert.True(discountId > 0);
    }

#endregion

#region GetDiscountsAsync

    [Fact]
    public async Task GetDiscountsAsync_ReturnsAllDiscounts_WhenNoFiltersAreProvided()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product { Name = "Laptop", CategoryId = category.Id };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        context.Discounts.Add(new Discount
        {
            Title = "Product Discount",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(3),
            ProductId = product.Id
        });

        context.Discounts.Add(new Discount
        {
            Title = "Category Discount",
            Percentage = 15,
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(4),
            CategoryId = category.Id
        });

        await context.SaveChangesAsync();

        var service = new DiscountService(context);

        var result = await service.GetDiscountsAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, discount => discount.Title == "Product Discount");
        Assert.Contains(result, discount => discount.Title == "Category Discount");
    }

    [Fact]
    public async Task GetDiscountsAsync_FiltersDiscountsByStartAndEndDate()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Discounts.Add(new Discount
        {
            Title = "Inside Range",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(7),
            CategoryId = category.Id
        });

        context.Discounts.Add(new Discount
        {
            Title = "Before Range",
            Percentage = 20,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            CategoryId = category.Id
        });

        context.Discounts.Add(new Discount
        {
            Title = "After Range",
            Percentage = 30,
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(12),
            CategoryId = category.Id
        });

        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var startDate = DateTime.UtcNow.AddDays(4);
        var endDate = DateTime.UtcNow.AddDays(8);

        var result = await service.GetDiscountsAsync(startDate, endDate);

        Assert.Single(result);
        Assert.Equal("Inside Range", result[0].Title);
    }

#endregion

#region GetDiscountByIdAsync

    [Fact]
    public async Task GetDiscountByIdAsync_ReturnsDiscount_WhenDiscountExists()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Books" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var discount = new Discount
        {
            Title = "Book Sale",
            Percentage = 25,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5),
            CategoryId = category.Id
        };
        context.Discounts.Add(discount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);

        var result = await service.GetDiscountByIdAsync(discount.Id);

        Assert.NotNull(result);
        Assert.Equal(discount.Id, result!.Id);
        Assert.Equal("Book Sale", result.Title);
        Assert.Equal(category.Id, result.CategoryId);
    }

    [Fact]
    public async Task GetDiscountByIdAsync_ThrowsKeyNotFoundException_WhenIdDoesNotExist()
    {
        using var context = CreateDbContext();

        var service = new DiscountService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetDiscountByIdAsync(999));
    }

#endregion

#region UpdateDiscountAsync

    [Fact]
    public async Task UpdateDiscountAsync_ThrowsInvalidOperationException_WhenChangingStartDateToPastDate()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var discount = new Discount
        {
            Title = "Launch Sale",
            Percentage = 15,
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(7),
            CategoryId = category.Id
        };
        context.Discounts.Add(discount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var dto = new AdminDiscountUpdateDto
        {
            Title = "Launch Sale Updated",
            Percentage = 15,
            StartDate = DateTime.UtcNow.AddMinutes(-10),
            EndDate = DateTime.UtcNow.AddDays(8),
            CategoryId = category.Id
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateDiscountAsync(discount.Id, dto));
    }

    [Fact]
    public async Task UpdateDiscountAsync_ThrowsInvalidOperationException_WhenChangingEndDateToPastDate()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var discount = new Discount
        {
            Title = "Weekend Sale",
            Percentage = 12,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5),
            CategoryId = category.Id
        };
        context.Discounts.Add(discount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var dto = new AdminDiscountUpdateDto
        {
            Title = "Weekend Sale Updated",
            Percentage = 12,
            StartDate = discount.StartDate,
            EndDate = DateTime.UtcNow.AddMinutes(-5),
            CategoryId = category.Id
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateDiscountAsync(discount.Id, dto));
    }

    [Fact]
    public async Task UpdateDiscountAsync_AllowsChangingOnlyTitleAndEndDate_WhenStartDateHasPassed()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Books" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var startedDiscount = new Discount
        {
            Title = "Book Spring Sale",
            Percentage = 20,
            StartDate = DateTime.UtcNow.AddDays(-2),
            EndDate = DateTime.UtcNow.AddDays(2),
            CategoryId = category.Id
        };
        context.Discounts.Add(startedDiscount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var dto = new AdminDiscountUpdateDto
        {
            Title = "Book Spring Sale Extended",
            Percentage = startedDiscount.Percentage,
            StartDate = startedDiscount.StartDate,
            EndDate = DateTime.UtcNow.AddDays(5),
            CategoryId = startedDiscount.CategoryId
        };

        await service.UpdateDiscountAsync(startedDiscount.Id, dto);
    }

    [Fact]
    public async Task UpdateDiscountAsync_ThrowsInvalidOperationException_WhenChangingPercentageAfterStartDate()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Gaming" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var startedDiscount = new Discount
        {
            Title = "Gaming Sale",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(3),
            CategoryId = category.Id
        };
        context.Discounts.Add(startedDiscount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var dto = new AdminDiscountUpdateDto
        {
            Title = startedDiscount.Title,
            Percentage = 35,
            StartDate = startedDiscount.StartDate,
            EndDate = startedDiscount.EndDate,
            CategoryId = startedDiscount.CategoryId
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateDiscountAsync(startedDiscount.Id, dto));
    }

    [Fact]
    public async Task UpdateDiscountAsync_ThrowsInvalidOperationException_WhenChangingProductIdAfterStartDate()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Hardware" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product1 = new Product { Name = "Mouse", CategoryId = category.Id };
        var product2 = new Product { Name = "Keyboard", CategoryId = category.Id };
        context.Products.AddRange(product1, product2);
        await context.SaveChangesAsync();

        var startedDiscount = new Discount
        {
            Title = "Accessory Discount",
            Percentage = 18,
            StartDate = DateTime.UtcNow.AddDays(-3),
            EndDate = DateTime.UtcNow.AddDays(2),
            ProductId = product1.Id
        };
        context.Discounts.Add(startedDiscount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var dto = new AdminDiscountUpdateDto
        {
            Title = startedDiscount.Title,
            Percentage = startedDiscount.Percentage,
            StartDate = startedDiscount.StartDate,
            EndDate = startedDiscount.EndDate,
            ProductId = product2.Id
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateDiscountAsync(startedDiscount.Id, dto));
    }

    [Fact]
    public async Task UpdateDiscountAsync_ThrowsInvalidOperationException_WhenBothProductAndCategoryProvided()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product { Name = "Laptop", CategoryId = category.Id };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var discount = new Discount
        {
            Title = "Initial Discount",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(6),
            ProductId = product.Id
        };
        context.Discounts.Add(discount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var dto = new AdminDiscountUpdateDto
        {
            Title = "Updated Discount",
            Percentage = 12,
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(7),
            ProductId = product.Id,
            CategoryId = category.Id
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateDiscountAsync(discount.Id, dto));
    }

    [Fact]
    public async Task UpdateDiscountAsync_ThrowsInvalidOperationException_WhenNeitherProductNorCategoryProvided()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Accessories" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var discount = new Discount
        {
            Title = "Initial Category Discount",
            Percentage = 14,
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(5),
            CategoryId = category.Id
        };
        context.Discounts.Add(discount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var dto = new AdminDiscountUpdateDto
        {
            Title = "Invalid Update",
            Percentage = 14,
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(6),
            ProductId = null,
            CategoryId = null
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateDiscountAsync(discount.Id, dto));
    }

    [Fact]
    public async Task UpdateDiscountAsync_ThrowsKeyNotFoundException_WhenProductDoesNotExist()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Computers" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var discount = new Discount
        {
            Title = "Category Promo",
            Percentage = 8,
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(5),
            CategoryId = category.Id
        };
        context.Discounts.Add(discount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var dto = new AdminDiscountUpdateDto
        {
            Title = "Switch To Missing Product",
            Percentage = 8,
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(7),
            ProductId = 9999,
            CategoryId = null
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateDiscountAsync(discount.Id, dto));
    }

    [Fact]
    public async Task UpdateDiscountAsync_ThrowsKeyNotFoundException_WhenCategoryDoesNotExist()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Displays" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product { Name = "Monitor", CategoryId = category.Id };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var discount = new Discount
        {
            Title = "Product Promo",
            Percentage = 9,
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(6),
            ProductId = product.Id
        };
        context.Discounts.Add(discount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var dto = new AdminDiscountUpdateDto
        {
            Title = "Switch To Missing Category",
            Percentage = 9,
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(8),
            ProductId = null,
            CategoryId = 9999
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateDiscountAsync(discount.Id, dto));
    }

    [Fact]
    public async Task UpdateDiscountAsync_ThrowsInvalidOperationException_WhenOverlappingProductDiscount()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Phones" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product { Name = "Smartphone", CategoryId = category.Id };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var discountToUpdate = new Discount
        {
            Title = "Early Discount",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            ProductId = product.Id
        };

        var existingDiscount = new Discount
        {
            Title = "Existing Product Discount",
            Percentage = 15,
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(8),
            ProductId = product.Id
        };

        context.Discounts.AddRange(discountToUpdate, existingDiscount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var dto = new AdminDiscountUpdateDto
        {
            Title = "Now Overlapping",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(6),
            EndDate = DateTime.UtcNow.AddDays(9),
            ProductId = product.Id,
            CategoryId = null
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateDiscountAsync(discountToUpdate.Id, dto));
    }

    [Fact]
    public async Task UpdateDiscountAsync_ThrowsInvalidOperationException_WhenOverlappingCategoryDiscount()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Networking" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var discountToUpdate = new Discount
        {
            Title = "Early Category Discount",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            CategoryId = category.Id
        };

        var existingDiscount = new Discount
        {
            Title = "Existing Category Discount",
            Percentage = 12,
            StartDate = DateTime.UtcNow.AddDays(4),
            EndDate = DateTime.UtcNow.AddDays(7),
            CategoryId = category.Id
        };

        context.Discounts.AddRange(discountToUpdate, existingDiscount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var dto = new AdminDiscountUpdateDto
        {
            Title = "Overlapping Category Update",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(8),
            ProductId = null,
            CategoryId = category.Id
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateDiscountAsync(discountToUpdate.Id, dto));
    }

    [Fact]
    public async Task UpdateDiscountAsync_ThrowsInvalidOperationException_WhenExistingDiscountWithoutEndDate_ForSameCategory()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Tablets" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var discountToUpdate = new Discount
        {
            Title = "Target Discount",
            Percentage = 8,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            CategoryId = category.Id
        };

        var existingOpenEnded = new Discount
        {
            Title = "Open Ended Discount",
            Percentage = 20,
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = null,
            CategoryId = category.Id
        };

        context.Discounts.AddRange(discountToUpdate, existingOpenEnded);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var dto = new AdminDiscountUpdateDto
        {
            Title = "Invalid Against Open Ended",
            Percentage = 8,
            StartDate = DateTime.UtcNow.AddDays(4),
            EndDate = DateTime.UtcNow.AddDays(6),
            ProductId = null,
            CategoryId = category.Id
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateDiscountAsync(discountToUpdate.Id, dto));
    }

    [Fact]
    public async Task UpdateDiscountAsync_AllowsProductAndCategoryDiscountsSamePeriod_WhenUpdating()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Cameras" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = new Product { Name = "Action Cam", CategoryId = category.Id };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var categoryDiscount = new Discount
        {
            Title = "Category Discount",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(4),
            EndDate = DateTime.UtcNow.AddDays(8),
            CategoryId = category.Id
        };

        var discountToUpdate = new Discount
        {
            Title = "Product Discount",
            Percentage = 5,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            ProductId = product.Id
        };

        context.Discounts.AddRange(categoryDiscount, discountToUpdate);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);
        var dto = new AdminDiscountUpdateDto
        {
            Title = "Updated Product Discount",
            Percentage = 5,
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(7),
            ProductId = product.Id,
            CategoryId = null
        };

        await service.UpdateDiscountAsync(discountToUpdate.Id, dto);
    }

#endregion

#region DeleteDiscountAsync

    [Fact]
    public async Task DeleteDiscountAsync_DeletesDiscount_WhenDiscountHasNotStartedYet()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var discount = new Discount
        {
            Title = "Upcoming Sale",
            Percentage = 10,
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(5),
            CategoryId = category.Id
        };
        context.Discounts.Add(discount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);

        await service.DeleteDiscountAsync(discount.Id);

        var deletedDiscount = await context.Discounts.FindAsync(discount.Id);

        Assert.Null(deletedDiscount);
    }

    [Fact]
    public async Task DeleteDiscountAsync_ThrowsInvalidOperationException_WhenDiscountHasStarted()
    {
        using var context = CreateDbContext();
        var category = new Category { Name = "Books" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var discount = new Discount
        {
            Title = "Active Sale",
            Percentage = 15,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(2),
            CategoryId = category.Id
        };
        context.Discounts.Add(discount);
        await context.SaveChangesAsync();

        var service = new DiscountService(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteDiscountAsync(discount.Id));
    }

    [Fact]
    public async Task DeleteDiscountAsync_ThrowsKeyNotFoundException_WhenIdDoesNotExist()
    {
        using var context = CreateDbContext();

        var service = new DiscountService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteDiscountAsync(999));
    }

#endregion

}