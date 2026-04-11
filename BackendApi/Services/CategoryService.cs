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

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    // ================= PUBLIC =================

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        return category == null ? null : new CategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}