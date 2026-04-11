using System.IO;
using System.Threading.Tasks;
using BackendApi.Dtos.Admin;
using BackendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        return product == null ? NotFound() : Ok(product);
    }

    [HttpGet("/api/admin/products")]
    public async Task<IActionResult> GetAdmin(int page = 1, string search = "")
    {
        return Ok(await _productService.GetAdminProductsAsync(page, search));
    }

    [HttpGet("/api/admin/products/{id}")]
    public async Task<IActionResult> GetAdminById(int id)
    {
        var product = await _productService.GetAdminProductByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost("/api/admin/products")]
    public async Task<IActionResult> Create(AdminProductCreateDto dto)
    {
        try
        {
            var id = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetAdminById), new { id }, new { id });
        }
        catch (FileNotFoundException ex)
        {
            return BadRequest(new { imagePath = ex.Message });
        }
    }

    [HttpPut("/api/admin/products/{id}")]
    public async Task<IActionResult> Update(int id, AdminProductUpdateDto dto)
    {
        return await _productService.UpdateProductAsync(id, dto)
            ? NoContent()
            : NotFound();
    }

    [HttpDelete("/api/admin/products/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _productService.DeleteProductAsync(id)
            ? NoContent()
            : NotFound();
    }

    [HttpGet("/api/admin/product-images")]
    public IActionResult Images()
    {
        return Ok(_productService.GetProductImages());
    }
}
