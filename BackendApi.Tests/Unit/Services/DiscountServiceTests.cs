using BackendApi.Data;
using BackendApi.DTOs;
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

}