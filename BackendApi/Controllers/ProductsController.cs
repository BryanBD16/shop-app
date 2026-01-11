using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendApi.DTOs;
using BackendApi.Dtos.Admin;
using System.Linq;
using BackendApi.Data;
using System.Threading.Tasks;
using System;
using System.IO;
using BackendApi.Models;
using System.Collections.Generic;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private const int PageSize = 12;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // PUBLIC ENDPOINTS
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] string search = "")
    {
        try
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

            var result = new PagedResultDto<ProductListItemDto>
            {
                Items = items,
                CurrentPage = page,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize)
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "An internal server error occurred.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products
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

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    // ==========================================
    // ADMIN ENDPOINTS
    // ==========================================
    [HttpGet("/api/admin/products")]
    public async Task<IActionResult> GetAdminProducts([FromQuery] int page = 1, [FromQuery] string search = "")
    {
        var query = _context.Products
                            .Where(p => p.Name.Contains(search));

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

        var result = new PagedResultDto<AdminProductListItemDto>
        {
            Items = items,
            CurrentPage = page,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize)
        };

        return Ok(result);
    }

    [HttpGet("/api/admin/products/{id}")]
    public async Task<IActionResult> GetAdminProductById(int id)
    {
        var product = await _context.Products
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

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPut("/api/admin/products/{id}")]
    public async Task<IActionResult> UpdateAdminProduct(int id, [FromBody] AdminProductUpdateDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.ImagePath = dto.ImagePath;
        product.Description = dto.Description;
        product.StockQuantity = dto.StockQuantity;
        product.IsPublished = dto.IsPublished;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("/api/admin/products")]
    public async Task<IActionResult> CreateAdminProduct([FromBody] AdminProductCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var physicalImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", dto.ImagePath.TrimStart('/'));
        if (!System.IO.File.Exists(physicalImagePath))
            return BadRequest(new { imagePath = "Selected product image does not exist." });

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

        return CreatedAtAction(nameof(GetAdminProductById), new { id = product.Id }, new { id = product.Id });
    }

    [HttpDelete("/api/admin/products/{id}")]
    public async Task<IActionResult> DeleteAdminProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound(new { message = "Product not found." });

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("/api/admin/product-images")]
    public IActionResult GetProductImages()
    {
        var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");

        if (!Directory.Exists(imageDir))
            return Ok(new List<string>());

        var images = Directory.GetFiles(imageDir)
            .Select(f => "/images/products/" + Path.GetFileName(f))
            .ToList();

        return Ok(images);
    }
}
