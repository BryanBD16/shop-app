using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BackendApi.Dtos.Admin;
using BackendApi.DTOs;
using BackendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{

    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        var categories = await _categoryService.GetCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        return category == null ? NotFound() : Ok(category);
    }   
}