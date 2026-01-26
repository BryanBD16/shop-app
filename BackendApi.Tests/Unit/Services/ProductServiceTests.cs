using BackendApi.Data;
using BackendApi.Models;
using BackendApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BackendApi.Tests.Unit.Services;

public class ProductServiceTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetPublishedProductsAsync_ReturnsOnlyPublishedProducts()
    {
        // Arrange
        using var context = CreateDbContext();
        context.Products.Add(new Product { Name = "Visible", IsPublished = true });
        context.Products.Add(new Product { Name = "Hidden", IsPublished = false });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act
        var result = await service.GetPublishedProductsAsync(1, "");

        // Assert
        Assert.Single(result.Items);
    }
}
