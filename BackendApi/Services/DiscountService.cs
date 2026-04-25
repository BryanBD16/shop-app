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

public class DiscountService : IDiscountService
{
    private readonly AppDbContext _context;

    public DiscountService(AppDbContext context)
    {
        _context = context;
    }

    // ================= PUBLIC =================


    // ================= ADMIN =================

    public Task<int> CreateDiscountAsync(AdminDiscountCreateDto dto)
    {
        validateProductAndCategory(dto.ProductId, dto.CategoryId);
        validateDateCreationAsync(dto.StartDate, dto.EndDate, dto.ProductId, dto.CategoryId);

        if(dto.ProductId != null)
        {
            var discount = new Discount
            {
                Title = dto.Title,
                Percentage = dto.Percentage,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ProductId = dto.ProductId
            };
            _context.Discounts.Add(discount);
            _context.SaveChanges();
            return Task.FromResult(discount.Id);
        }
        else if(dto.CategoryId != null)
        {
            var discount = new Discount
            {
                Title = dto.Title,
                Percentage = dto.Percentage,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CategoryId = dto.CategoryId
            };
            _context.Discounts.Add(discount);
            _context.SaveChanges();
            return Task.FromResult(discount.Id);
        }
        throw new InvalidOperationException("Either productId or categoryId must be provided");
    }

    public Task<bool> UpdateDiscountAsync(int id, AdminDiscountUpdateDto dto)
    {
        // TODO
        throw new NotImplementedException();
    }

    public Task<List<DiscountDto>> GetDiscountsAsync()
    {
        // TODO
        throw new NotImplementedException();
    }

    public Task<DiscountDto?> GetDiscountByIdAsync(int id)
    {
        // TODO
        throw new NotImplementedException();
    }

    public Task validateDateCreationAsync(DateTime startDate, DateTime? endDate, int? productId = null, int? categoryId = null)
    {
            var product = productId != null ? new Product { Id = productId.Value } : null;
            var category = categoryId != null ? new Category { Id = categoryId.Value } : null;
        if (startDate < DateTime.UtcNow)
            throw new InvalidOperationException("Start date cannot be in the past");

        if (endDate <= startDate)
            throw new InvalidOperationException("End date must be after start date");

        if(product != null)
        {
            var overlappingProductDiscount = _context.Discounts
                .Where(d => d.ProductId == product.Id)
                .Where(d => (d.StartDate < endDate && d.EndDate > startDate )|| (d.EndDate == null && (d.StartDate < endDate || endDate == null)))
                .FirstOrDefault();

            if (overlappingProductDiscount != null)
                throw new InvalidOperationException("There is already an overlapping discount for this product");
        }

        if(category != null)
        {
            var overlappingCategoryDiscount = _context.Discounts
                .Where(d => d.CategoryId == category.Id)
                .Where(d => (d.StartDate < endDate && d.EndDate > startDate )|| (d.EndDate == null && (d.StartDate < endDate || endDate == null)))
                .FirstOrDefault();

            if (overlappingCategoryDiscount != null)
                throw new InvalidOperationException("There is already an overlapping discount for this category");
        }

        return Task.CompletedTask;
    }

    public Task validateProductAndCategory(int? productId, int? categoryId)
    {
        var product = productId != null ? new Product { Id = productId.Value } : null;
        var category = categoryId != null ? new Category { Id = categoryId.Value } : null;

        if (product == null && category == null)
            throw new InvalidOperationException("Either product or category must be provided");

        if (product != null && category != null)
            throw new InvalidOperationException("Cannot provide both product and category");

        if(product != null)
        {
            var existingProduct = _context.Products.Find(product.Id);
            if (existingProduct == null)
                throw new KeyNotFoundException("Product not found");
        }
        if(category != null)
        {
            var existingCategory = _context.Categories.Find(category.Id);
            if (existingCategory == null)
                throw new KeyNotFoundException("Category not found");
        }
        return Task.CompletedTask;
    }
}