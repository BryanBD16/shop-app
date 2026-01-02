using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using BackendApi.Models;
using System.Collections.Generic;

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
    public List<Product> Get()
    {
        var products = new List<Product>();

        using var conn = new MySqlConnection(_connectionString);
        conn.Open();

        var cmd = new MySqlCommand("SELECT * FROM products", conn);
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

        return products;
    }
}
