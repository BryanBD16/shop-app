using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using BackendApi.Models;
using System.Collections.Generic;
using System;

namespace BackendApi.Controllers;


[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly string _connectionString;

    public ProductsController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    [HttpGet]
    public IActionResult Get([FromQuery] int page = 1, [FromQuery] string search = "")
    {
        try
        {
            var products = new List<Product>();
            int pageSize = 12;
            int offset = (page - 1) * pageSize;

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(
                "SELECT * FROM products WHERE name LIKE @search ORDER BY id LIMIT @limit OFFSET @offset", conn
            );
            cmd.Parameters.AddWithValue("@search", "%" + search + "%");
            cmd.Parameters.AddWithValue("@limit", pageSize);
            cmd.Parameters.AddWithValue("@offset", offset);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Price = reader.GetDecimal("price")
                });
            }

            return Ok(products);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An internal server error occurred.");
        }
    }

}
