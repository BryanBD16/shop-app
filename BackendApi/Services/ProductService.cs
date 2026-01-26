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
    public async Task<PagedResultDto<ProductListItemDto>> GetPublishedProductsAsync(int page, string search)
    {
        var query = _context.Products
            .Where(p => p.IsPublished && p.Name.Contains(search));

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImagePath = p.ImagePath
            })
            .ToListAsync();

        return new PagedResultDto<ProductListItemDto>
        {
            Items = items,
            CurrentPage = page,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize)
        };
    }

    public async Task<ProductDetailsDto?> GetPublishedProductByIdAsync(int id)
    {
        return await _context.Products
            .Where(p => p.IsPublished && p.Id == id)
            .Select(p => new ProductDetailsDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImagePath = p.ImagePath,
                Description = p.Description
            })
            .FirstOrDefaultAsync();
    }

    // ================= ADMIN =================
    public async Task<PagedResultDto<AdminProductListItemDto>> GetAdminProductsAsync(int page, string search)
    {
        var query = _context.Products.Where(p => p.Name.Contains(search));
        var totalItems = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(p => new AdminProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImagePath = p.ImagePath,
                StockQuantity = p.StockQuantity,
                IsPublished = p.IsPublished
            })
            .ToListAsync();

        return new PagedResultDto<AdminProductListItemDto>
        {
            Items = items,
            CurrentPage = page,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize)
        };
    }

    public async Task<AdminProductDetailsDto?> GetAdminProductByIdAsync(int id)
    {
        return await _context.Products
            .Where(p => p.Id == id)
            .Select(p => new AdminProductDetailsDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImagePath = p.ImagePath,
                Description = p.Description,
                StockQuantity = p.StockQuantity,
                IsPublished = p.IsPublished
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateProductAsync(AdminProductCreateDto dto)
    {
        var imagePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            dto.ImagePath.TrimStart('/')
        );

        if (!File.Exists(imagePath))
            throw new FileNotFoundException("Image does not exist.");

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            ImagePath = dto.ImagePath,
            Description = dto.Description,
            StockQuantity = dto.StockQuantity,
            IsPublished = dto.IsPublished
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product.Id;
    }

    public async Task<bool> UpdateProductAsync(int id, AdminProductUpdateDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.ImagePath = dto.ImagePath;
        product.Description = dto.Description;
        product.StockQuantity = dto.StockQuantity;
        product.IsPublished = dto.IsPublished;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
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
}
