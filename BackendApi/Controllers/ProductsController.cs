using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BackendApi.Dtos.Admin;
using BackendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int page = 1, string search = "", int? categoryId = null)
    {
        return Ok(await _productService.GetPublishedProductsAsync(page, search, categoryId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetPublishedProductByIdAsync(id);
        return  Ok(product);
    }

    [HttpGet("/api/admin/products")]
    public async Task<IActionResult> GetAdmin(int page = 1, string search = "", int? categoryId = null)
    {
        return Ok(await _productService.GetAdminProductsAsync(page, search, categoryId));
    }

    [HttpGet("/api/admin/products/{id}")]
    public async Task<IActionResult> GetAdminById(int id)
    {
        var product = await _productService.GetAdminProductByIdAsync(id);
        return  Ok(product);
    }

    [HttpPost("/api/admin/products")]
    public async Task<IActionResult> Create(AdminProductCreateDto dto)
    {

         var id = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetAdminById), new { id }, new { id });

    }

    [HttpPut("/api/admin/products/{id}")]
    public async Task<IActionResult> Update(int id, AdminProductUpdateDto dto)
    {
        await _productService.UpdateProductAsync(id, dto);
        return NoContent();
    }

    [HttpGet("/api/admin/product-images")]
    public IActionResult Images()
    {
        return Ok(_productService.GetProductImages());
    }
}
