using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Dtos.Admin;
using BackendApi.Models;
using BackendApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BackendApi.Tests.Unit.Services;

public class CategoryServiceTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

#region GetCategoriesAsync

    [Fact]
    public async Task GetCategoriesAsync_ReturnsAllCategories()
    {
        using var context = CreateDbContext();

        context.Categories.Add(new Category { Name = "Electronics" });
        context.Categories.Add(new Category { Name = "Books" });
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        var result = await service.GetCategoriesAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Name == "Electronics");
        Assert.Contains(result, c => c.Name == "Books");
    }

    [Fact]
    public async Task GetCategoriesAsync_ReturnsEmptyList_WhenNoCategoriesExist()
    {
        using var context = CreateDbContext();

        var service = new CategoryService(context);

        var result = await service.GetCategoriesAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

#endregion

#region GetCategoryByIdAsync

    [Fact]
    public async Task GetCategoryByIdAsync_ReturnsCategory_WhenCategoryExists()
    {
        using var context = CreateDbContext();

        var category = new Category { Name = "Electronics" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        var result = await service.GetCategoryByIdAsync(category.Id);

        Assert.NotNull(result);
        Assert.Equal(category.Id, result!.Id);
        Assert.Equal("Electronics", result.Name);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ReturnsNull_WhenIdDoesNotExist()
    {
        using var context = CreateDbContext();

        context.Categories.Add(new Category { Name = "Books" });
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        var result = await service.GetCategoryByIdAsync(999);

        Assert.Null(result);
    }

#endregion

#region CreateCategoryAsync

    [Fact]
    public async Task CreateCategoryAsync_CreatesCategoryAndReturnsId()
    {
        using var context = CreateDbContext();

        var service = new CategoryService(context);
        var dto = new AdminCategoryCreateDto { Name = "Audio" };

        var createdId = await service.CreateCategoryAsync(dto);

        var createdCategory = await context.Categories.FindAsync(createdId);

        Assert.True(createdId > 0);
        Assert.NotNull(createdCategory);
        Assert.Equal("Audio", createdCategory!.Name);
    }

    [Fact]
    public async Task CreateCategoryAsync_AcceptsNameAtMaxLengthLimit()
    {
        using var context = CreateDbContext();

        var service = new CategoryService(context);
        var dto = new AdminCategoryCreateDto { Name = new string('A', 255) };

        var createdId = await service.CreateCategoryAsync(dto);

        var createdCategory = await context.Categories.FindAsync(createdId);

        Assert.NotNull(createdCategory);
        Assert.Equal(255, createdCategory!.Name.Length);
    }

#endregion

#region UpdateCategoryAsync

    [Fact]
    public async Task UpdateCategoryAsync_ReturnsTrueAndUpdatesCategory_WhenCategoryExists()
    {
        using var context = CreateDbContext();

        var category = new Category { Name = "Old Name" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);
        var dto = new AdminCategoryUpdateDto { Name = "New Name" };

        var updated = await service.UpdateCategoryAsync(category.Id, dto);
        var updatedCategory = await context.Categories.FindAsync(category.Id);

        Assert.True(updated);
        Assert.NotNull(updatedCategory);
        Assert.Equal("New Name", updatedCategory!.Name);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ReturnsFalse_WhenCategoryDoesNotExist()
    {
        using var context = CreateDbContext();

        var service = new CategoryService(context);
        var dto = new AdminCategoryUpdateDto { Name = "New Name" };

        var updated = await service.UpdateCategoryAsync(999, dto);

        Assert.False(updated);
    }

    [Fact]
    public async Task UpdateCategoryAsync_AcceptsNameAtMaxLengthLimit()
    {
        using var context = CreateDbContext();

        var category = new Category { Name = "Initial" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);
        var dto = new AdminCategoryUpdateDto { Name = new string('B', 255) };

        var updated = await service.UpdateCategoryAsync(category.Id, dto);
        var updatedCategory = await context.Categories.FindAsync(category.Id);

        Assert.True(updated);
        Assert.NotNull(updatedCategory);
        Assert.Equal(255, updatedCategory!.Name.Length);
    }

#endregion

#region DeleteCategoryAsync

    [Fact]
    public async Task DeleteCategoryAsync_ReturnsTrueAndDeletesCategory_WhenCategoryExists()
    {
        using var context = CreateDbContext();

        var category = new Category { Name = "To Delete" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        var deleted = await service.DeleteCategoryAsync(category.Id);
        var deletedCategory = await context.Categories.FindAsync(category.Id);

        Assert.True(deleted);
        Assert.Null(deletedCategory);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ReturnsFalse_WhenCategoryDoesNotExist()
    {
        using var context = CreateDbContext();

        var service = new CategoryService(context);

        var deleted = await service.DeleteCategoryAsync(999);

        Assert.False(deleted);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ThrowsInvalidOperationException_WhenCategoryHasProducts()
    {
        using var context = CreateDbContext();

        var category = new Category
        {
            Name = "Electronics",
            Products = new List<Product>
            {
                new Product
                {
                    Name = "Headphones"
                }
            }
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteCategoryAsync(category.Id));
    }

#endregion
}
