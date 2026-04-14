using BackendApi.Data;
using BackendApi.Models;
using BackendApi.Services;
using Microsoft.EntityFrameworkCore;
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
}
