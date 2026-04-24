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
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            throw new KeyNotFoundException("Category not found");

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    //category name has to be unique
    public async Task<int> CreateCategoryAsync(AdminCategoryCreateDto dto)
    {
        var category = new Category
        {
            Name = dto.Name
        };

        var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category.Name);
        if (existingCategory != null)
        {
            throw new InvalidOperationException("Category name already exists.");
        }

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return category.Id;
    }

    public async Task UpdateCategoryAsync(int id, AdminCategoryUpdateDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) throw new KeyNotFoundException($"Category with id {id} not found.");

        category.Name = dto.Name;

        var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == dto.Name && c.Id != id);
        if (existingCategory != null)
        {
            throw new InvalidOperationException("Category name already exists.");
        }

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        return;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories    
                                        .Include(c => c.Products)
                                        .FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) throw new KeyNotFoundException($"Category with id {id} not found.");
        if (category.Products != null && category.Products.Any())
        {
            throw new InvalidOperationException("Cannot delete category with associated products.");
        }
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return;
    }
}