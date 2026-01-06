using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using BackendApi.DTOs;
using System;
using System.Collections.Generic;

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
}
