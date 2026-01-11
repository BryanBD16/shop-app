using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using BackendApi.DTOs;
using System;
using System.Collections.Generic;
using BackendApi.Dtos.Admin;
using System.IO;
using System.Linq;
using BackendApi.Dtos;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly string _connectionString;
    private const int PageSize = 12;

    public ProductsController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    [HttpGet]
    public IActionResult Get([FromQuery] int page = 1, [FromQuery] string search = "")
    {
        try
        {
            var items = new List<ProductListItemDto>();
            int offset = (page - 1) * PageSize;
            int totalItems;

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // 🔹 Count total products
            using (var countCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM products WHERE name LIKE @search AND is_published = 1",
                conn))
            {
                countCmd.Parameters.AddWithValue("@search", $"%{search}%");
                totalItems = Convert.ToInt32(countCmd.ExecuteScalar());
            }

            // 🔹 Get paged products
            using (var cmd = new MySqlCommand(
                @"SELECT id, name, price, image_path
                  FROM products
                  WHERE name LIKE @search AND is_published = 1
                  ORDER BY id
                  LIMIT @limit OFFSET @offset",
                conn))
            {
                cmd.Parameters.AddWithValue("@search", $"%{search}%");
                cmd.Parameters.AddWithValue("@limit", PageSize);
                cmd.Parameters.AddWithValue("@offset", offset);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new ProductListItemDto
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Price = reader.GetDecimal("price"),
                        ImagePath = reader.GetString("image_path")
                    });
                }
            }

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
    public IActionResult GetById(int id)
    {
        try
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(
                @"SELECT id, name, price, image_path, description
                FROM products
                WHERE id = @id AND is_published = 1",
                conn
            );
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return NotFound();

            var product = new ProductDetailsDto
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Price = reader.GetDecimal("price"),
                ImagePath = reader.GetString("image_path"),
                Description = reader.GetString("description")
            };

            return Ok(product);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "An internal server error occurred.");
        }
    }
    // ==========================================
    // ADMIN ENDPOINTS
    // ==========================================

    // [Authorize(Roles = "Admin")]
    [HttpGet("/api/admin/products")]
    public IActionResult GetAdminProducts([FromQuery] int page = 1, [FromQuery] string search = "")
    {
        try
        {
            var items = new List<AdminProductListItemDto>();
            int offset = (page - 1) * PageSize;
            int totalItems;

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // Count total products (admin sees all)
            using (var countCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM products WHERE name LIKE @search",
                conn))
            {
                countCmd.Parameters.AddWithValue("@search", $"%{search}%");
                totalItems = Convert.ToInt32(countCmd.ExecuteScalar());
            }

            // Get paged products
            using (var cmd = new MySqlCommand(
                @"SELECT id, name, price, image_path, stock_quantity, is_published
                FROM products
                WHERE name LIKE @search
                ORDER BY id
                LIMIT @limit OFFSET @offset",
                conn))
            {
                cmd.Parameters.AddWithValue("@search", $"%{search}%");
                cmd.Parameters.AddWithValue("@limit", PageSize);
                cmd.Parameters.AddWithValue("@offset", offset);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new AdminProductListItemDto
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Price = reader.GetDecimal("price"),
                        ImagePath = reader.GetString("image_path"),
                        StockQuantity = reader.GetInt32("stock_quantity"),
                        IsPublished = reader.GetBoolean("is_published")
                    });
                }
            }

            var result = new PagedResultDto<AdminProductListItemDto>
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

    // [Authorize(Roles = "Admin")]
    [HttpGet("/api/admin/products/{id}")]
    public IActionResult GetAdminProductById(int id)
    {
        try
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(
                @"SELECT id, name, price, image_path, description, stock_quantity, is_published
                FROM products
                WHERE id = @id",
                conn
            );
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return NotFound();

            var product = new AdminProductDetailsDto
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Price = reader.GetDecimal("price"),
                ImagePath = reader.GetString("image_path"),
                Description = reader.GetString("description"),
                StockQuantity = reader.GetInt32("stock_quantity"),
                IsPublished = reader.GetBoolean("is_published")
            };

            return Ok(product);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "An internal server error occurred.");
        }
    }

    // [Authorize(Roles = "Admin")]
    [HttpPut("/api/admin/products/{id}")]
    public IActionResult UpdateAdminProduct(int id, [FromBody] AdminProductUpdateDto dto)
    {
        try
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // Vérifier si le produit existe
            using (var checkCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM products WHERE id = @id",
                conn))
            {
                checkCmd.Parameters.AddWithValue("@id", id);
                var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

                if (!exists)
                    return NotFound();
            }

            // Update du produit
            using var cmd = new MySqlCommand(
                @"UPDATE products
                SET
                    name = @name,
                    price = @price,
                    image_path = @imagePath,
                    description = @description,
                    stock_quantity = @stockQuantity,
                    is_published = @isPublished
                WHERE id = @id",
                conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", dto.Name);
            cmd.Parameters.AddWithValue("@price", dto.Price);
            cmd.Parameters.AddWithValue("@imagePath", dto.ImagePath);
            cmd.Parameters.AddWithValue("@description", dto.Description);
            cmd.Parameters.AddWithValue("@stockQuantity", dto.StockQuantity);
            cmd.Parameters.AddWithValue("@isPublished", dto.IsPublished);

            cmd.ExecuteNonQuery();

            return NoContent(); // 204 → standard REST pour update réussi
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "An internal server error occurred.");
        }
    }

    // [Authorize(Roles = "Admin")]
    [HttpGet("/api/admin/product-images")]
    public IActionResult GetProductImages()
    {
        try
        {
            var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");

            if (!Directory.Exists(imageDir))
                return Ok(new List<string>());

            var images = Directory.GetFiles(imageDir)
                .Select(f => "/images/products/" + Path.GetFileName(f))
                .ToList();

            return Ok(images);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "Failed to load images");
        }
    }

   // [Authorize(Roles = "Admin")]
    [HttpPost("/api/admin/products")]
    public IActionResult CreateAdminProduct([FromBody] AdminProductCreateDto dto)
    {
        // DTO validation (Data Annotations)
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                errors = ModelState.ToDictionary(
                    e => e.Key,
                    e => e.Value.Errors.First().ErrorMessage
                )
            });
        }

        // Business validation: image required
        if (string.IsNullOrWhiteSpace(dto.ImagePath))
        {
            return BadRequest(new
            {
                errors = new
                {
                    imagePath = "Product image is required."
                }
            });
        }

        // Business validation: image must exist on server
        var physicalImagePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            dto.ImagePath.TrimStart('/')
        );

        if (!System.IO.File.Exists(physicalImagePath))
        {
            return BadRequest(new
            {
                errors = new
                {
                    imagePath = "Selected product image does not exist."
                }
            });
        }

        // Database insert
        try
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            using var cmd = new MySqlCommand(
                @"INSERT INTO products
                    (name, price, image_path, description, stock_quantity, is_published)
                VALUES
                    (@name, @price, @imagePath, @description, @stockQuantity, @isPublished);
                SELECT LAST_INSERT_ID();",
                conn
            );

            cmd.Parameters.AddWithValue("@name", dto.Name);
            cmd.Parameters.AddWithValue("@price", dto.Price);
            cmd.Parameters.AddWithValue("@imagePath", dto.ImagePath);
            cmd.Parameters.AddWithValue("@description", dto.Description);
            cmd.Parameters.AddWithValue("@stockQuantity", dto.StockQuantity);
            cmd.Parameters.AddWithValue("@isPublished", dto.IsPublished);

            var newProductId = Convert.ToInt32(cmd.ExecuteScalar());

            return CreatedAtAction(
                nameof(GetAdminProductById),
                new { id = newProductId },
                new { id = newProductId }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new
            {
                message = "An internal server error occurred."
            });
        }
    }

    // [Authorize(Roles = "Admin")]
    [HttpDelete("/api/admin/products/{id}")]
    public IActionResult DeleteAdminProduct(int id)
    {
        try
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // Check if product exists
            using (var checkCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM products WHERE id = @id",
                conn))
            {
                checkCmd.Parameters.AddWithValue("@id", id);

                var exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (exists == 0)
                    return NotFound(new { message = "Product not found." });
            }

            // Delete product
            using var deleteCmd = new MySqlCommand(
                "DELETE FROM products WHERE id = @id",
                conn
            );
            deleteCmd.Parameters.AddWithValue("@id", id);

            deleteCmd.ExecuteNonQuery();

            return NoContent(); // 204
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new
            {
                message = "An internal server error occurred."
            });
        }
    }


}
