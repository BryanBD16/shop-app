using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Dtos.Admin;
using BackendApi.Models;
using BackendApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendApi.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private const int PageSize = 12;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    // ================= PUBLIC =================
    public async Task<PagedResultDto<ProductListItemDto>> GetPublishedProductsAsync(int page, string search, int? categoryId = null)
    {
        var query = _context.Products
            .Where(p => p.IsPublished);

        if (search != null)
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                OriginalPrice = p.Price,
                ImagePath = p.ImagePath,
                CategoryId = p.CategoryId
            })
            .ToListAsync();

        await ApplyDiscountsAsync(
            items,
            i => i.Id,
            i => i.CategoryId,
            i => i.OriginalPrice,
            (i, price) => i.DiscountedPrice = price
        );

        return new PagedResultDto<ProductListItemDto>
        {
            Items = items,
            CurrentPage = page,
            TotalItems = totalItems,
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)PageSize))
        };
    }

    public async Task<ProductDetailsDto> GetPublishedProductByIdAsync(int id)
    {
        var product = await _context.Products
          .FirstOrDefaultAsync(p => p.IsPublished && p.Id == id) ?? throw new KeyNotFoundException("Product with " + id + " not found.");
        var discounted = await GetDiscountedPriceAsync(product);
        return new ProductDetailsDto
        {
            Id = product.Id,
            Name = product.Name,
            OriginalPrice = product.Price,
            DiscountedPrice = discounted,
            ImagePath = product.ImagePath,
            Description = product.Description,
            CategoryId = product.CategoryId
        };
    }

    // ================= ADMIN =================
    public async Task<PagedResultDto<AdminProductListItemDto>> GetAdminProductsAsync(int page, string search, int? categoryId = null)
    {
        var query = _context.Products.AsQueryable();

        if (search != null)
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(p => new AdminProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                OriginalPrice = p.Price,
                ImagePath = p.ImagePath,
                StockQuantity = p.StockQuantity,
                IsPublished = p.IsPublished,
                CategoryId = p.CategoryId
            })
            .ToListAsync();

        await ApplyDiscountsAsync(
            items,
            i => i.Id,
            i => i.CategoryId,
            i => i.OriginalPrice,
            (i, price) => i.DiscountedPrice = price
        );

        return new PagedResultDto<AdminProductListItemDto>
        {
            Items = items,
            CurrentPage = page,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize)
        };
    }

    public async Task<AdminProductDetailsDto> GetAdminProductByIdAsync(int id)
    {
        var product = await _context.Products
          .FirstOrDefaultAsync(p => p.Id == id) ?? throw new KeyNotFoundException("Product with " + id + " not found.");
        var discounted = await GetDiscountedPriceAsync(product);
        return new AdminProductDetailsDto
            {
                Id = product.Id,
                Name = product.Name,
                OriginalPrice = product.Price,
                ImagePath = product.ImagePath,
                Description = product.Description,
                StockQuantity = product.StockQuantity,
                IsPublished = product.IsPublished,
                CategoryId = product.CategoryId
            };
    }

    private async Task ValidateCategory(int categoryId)
    {
        var exists = await _context.Categories.AnyAsync(c => c.Id == categoryId);

        if (!exists)
            throw new InvalidOperationException("Invalid category");
    }

    public async Task<int> CreateProductAsync(AdminProductCreateDto dto)
    {
        var imagePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            dto.ImagePath.TrimStart('/')
        );

        if (!File.Exists(imagePath))
            throw new InvalidOperationException("Image does not exist.");

        await ValidateCategory(dto.CategoryId);

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            ImagePath = dto.ImagePath,
            Description = dto.Description,
            StockQuantity = dto.StockQuantity,
            IsPublished = dto.IsPublished,
            CategoryId = dto.CategoryId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product.Id;
    }

    public async Task UpdateProductAsync(int id, AdminProductUpdateDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) throw new KeyNotFoundException("Product with " + id + " not found.");

        await ValidateCategory(dto.CategoryId);

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.ImagePath = dto.ImagePath;
        product.Description = dto.Description;
        product.StockQuantity = dto.StockQuantity;
        product.IsPublished = dto.IsPublished;
        product.CategoryId = dto.CategoryId;
        await _context.SaveChangesAsync();
        return;
    }

    public List<string> GetProductImages()
    {
        var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");

        if (!Directory.Exists(imageDir))
            return new List<string>();

        return Directory.GetFiles(imageDir)
            .Select(f => "/images/products/" + Path.GetFileName(f))
            .ToList();
    }

    private async Task<decimal?> GetDiscountedPriceAsync(Product product)
    {
        var now = DateTime.UtcNow;

        var discount = await _context.Discounts
            .Where(d =>
                (d.ProductId == product.Id || d.CategoryId == product.CategoryId) &&
                d.StartDate <= now &&
                (d.EndDate == null || d.EndDate > now)
            )
            .OrderByDescending(d => d.Percentage)
            .FirstOrDefaultAsync();

        if (discount == null)
            return null;

        return product.Price - (product.Price * discount.Percentage / 100);
    }

    private async Task ApplyDiscountsAsync<T>(
    List<T> items,
    Func<T, int> getProductId,
    Func<T, int> getCategoryId,
    Func<T, decimal> getPrice,
    Action<T, decimal> setDiscountedPrice)
{
    var productIds = items.Select(getProductId).ToList();
    var categoryIds = items.Select(getCategoryId).Distinct().ToList();

    var now = DateTime.UtcNow;

    var discounts = await _context.Discounts
        .Where(d =>
            (d.ProductId != null && productIds.Contains(d.ProductId.Value)) ||
            (d.CategoryId != null && categoryIds.Contains(d.CategoryId.Value))
        )
        .Where(d => d.StartDate <= now && (d.EndDate == null || d.EndDate > now))
        .ToListAsync();

    foreach (var item in items)
    {
        var productDiscount = discounts
            .Where(d => d.ProductId == getProductId(item));

        var categoryDiscount = discounts
            .Where(d => d.CategoryId == getCategoryId(item));

        var bestDiscount = productDiscount
            .Concat(categoryDiscount)
            .OrderByDescending(d => d.Percentage)
            .FirstOrDefault();

        if (bestDiscount != null)
        {
            var price = getPrice(item);
            var discounted = price - (price * bestDiscount.Percentage / 100);

            setDiscountedPrice(item, discounted);
        }
    }
}
}
