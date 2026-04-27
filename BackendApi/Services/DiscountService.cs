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
using System.Linq.Expressions;

namespace BackendApi.Services;

public class DiscountService : IDiscountService
{
    private readonly AppDbContext _context;

    public DiscountService(AppDbContext context)
    {
        _context = context;
    }

    // ================= PUBLIC =================

    public async Task<List<DiscountDto>> GetDiscountsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Discounts.AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(d => d.StartDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(d => d.EndDate <= endDate.Value);
        }

        var discounts = await query
            .Select(d => new DiscountDto
            {
                Id = d.Id,
                Title = d.Title,
                Percentage = d.Percentage,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                ProductId = d.ProductId,
                CategoryId = d.CategoryId
            })
            .ToListAsync();

        return discounts;
    }

    public async Task<DiscountDto> GetDiscountByIdAsync(int id)
    {
        var discount = await _context.Discounts
            .Where(d => d.Id == id)
            .Select(d => new DiscountDto
            {
                Id = d.Id,
                Title = d.Title,
                Percentage = d.Percentage,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                ProductId = d.ProductId,
                CategoryId = d.CategoryId
            })
            .FirstOrDefaultAsync();
        return discount ?? throw new KeyNotFoundException("Discount not found");
    }

    // ================= ADMIN =================

    public async Task<int> CreateDiscountAsync(AdminDiscountCreateDto dto)
    {
        await ValidateProductAndCategoryAsync(dto.ProductId, dto.CategoryId);
        await ValidateDiscountDatesAsync(dto.StartDate, dto.EndDate, dto.ProductId, dto.CategoryId);

        
        var discount = new Discount
        {
            Title = dto.Title,
            Percentage = dto.Percentage,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ProductId = dto.ProductId,
            CategoryId = dto.CategoryId
        };
        _context.Discounts.Add(discount);
        await _context.SaveChangesAsync();
        return discount.Id;
    }

    public async Task UpdateDiscountAsync(int id, AdminDiscountUpdateDto dto)
    {
        Discount discount = await GetDiscountOrThrowAsync(id);

        var now = DateTime.UtcNow;

        await ValidateProductAndCategoryAsync(dto.ProductId, dto.CategoryId);

        if (discount.EndDate <= now && discount.EndDate != null)
        {
            throw new InvalidOperationException("Cannot update a discount that has already ended");
        }
        else if (discount.StartDate <= now && (discount.EndDate == null || discount.EndDate > now))
        {
            if (dto.StartDate != discount.StartDate || dto.ProductId != discount.ProductId || dto.CategoryId != discount.CategoryId || dto.Percentage != discount.Percentage)
                throw new InvalidOperationException("Cannot change discount information after it has started, except for end date and title");

            await ValidateDateUpdateAsync(
                dto.StartDate,
                dto.EndDate,
                dto.ProductId,
                dto.CategoryId,
                id
            );

            discount.EndDate = dto.EndDate;
            discount.Title = dto.Title;
        }
        else if (discount.StartDate > now)
        {
            await ValidateDiscountDatesAsync(
                dto.StartDate,
                dto.EndDate,
                dto.ProductId,
                dto.CategoryId,
                id
            );
            discount.StartDate = dto.StartDate;
            discount.EndDate = dto.EndDate;
            discount.Percentage = dto.Percentage;
            discount.Title = dto.Title;
            discount.ProductId = dto.ProductId;
            discount.CategoryId = dto.CategoryId;
        }

        await _context.SaveChangesAsync();
    }

    private async Task<Discount> GetDiscountOrThrowAsync(int id)
    {
        var discount = await _context.Discounts
            .Where(d => d.Id == id)
            .FirstOrDefaultAsync();

        if (discount == null)
            throw new KeyNotFoundException("Discount not found");
        return discount;
    }

    private async Task ValidateDateUpdateAsync(
        DateTime startDate,
        DateTime? endDate,
        int? productId = null,
        int? categoryId = null,
        int? excludeId = null)
    {
        var now = DateTime.UtcNow;
        
        if (endDate.HasValue && endDate <= now)
            throw new InvalidOperationException("End date must be in the future");

        if (productId != null)
        {
            var exists = await HasOverlappingDiscountAsync(
                d => d.ProductId == productId,
                startDate,
                endDate,
                excludeId
            );

            if (exists)
                throw new InvalidOperationException("There is already an overlapping discount for this product");
        }

        if (categoryId != null)
        {
            var exists = await HasOverlappingDiscountAsync(
                d => d.CategoryId == categoryId,
                startDate,
                endDate,
                excludeId
            );

            if (exists)
                throw new InvalidOperationException("There is already an overlapping discount for this category");
        }
    }

    private async Task ValidateDiscountDatesAsync(
        DateTime startDate,
        DateTime? endDate,
        int? productId = null,
        int? categoryId = null,
        int? excludeId = null)
    {
        var now = DateTime.UtcNow;

        if (startDate < now)
            throw new InvalidOperationException("Start date cannot be in the past");

        if (endDate.HasValue && endDate <= startDate)
            throw new InvalidOperationException("End date must be after start date");

        if (productId != null)
        {
            var exists = await HasOverlappingDiscountAsync(
                d => d.ProductId == productId,
                startDate,
                endDate,
                excludeId
            );

            if (exists)
                throw new InvalidOperationException("There is already an overlapping discount for this product");
        }

        if (categoryId != null)
        {
            var exists = await HasOverlappingDiscountAsync(
                d => d.CategoryId == categoryId,
                startDate,
                endDate,
                excludeId
            );

            if (exists)
                throw new InvalidOperationException("There is already an overlapping discount for this category");
        }
    }

    private async Task<bool> HasOverlappingDiscountAsync(
        Expression<Func<Discount, bool>> filter,
        DateTime newStart,
        DateTime? newEnd,
        int? excludeId = null)
        {
            var query = _context.Discounts.Where(filter);

            if (excludeId.HasValue)
            {
                query = query.Where(d => d.Id != excludeId.Value);
            }

            return await query
                .Where(d =>
                    (d.EndDate == null || newStart < d.EndDate) &&
                    (newEnd == null || d.StartDate < newEnd)
                )
                .AnyAsync();
        }

    private async Task ValidateProductAndCategoryAsync(int? productId, int? categoryId)
    {
        if (productId == null && categoryId == null)
            throw new InvalidOperationException("Either product or category must be provided");

        if (productId != null && categoryId != null)
            throw new InvalidOperationException("Cannot provide both product and category");

        if(productId != null)
        {
            var exists = await _context.Products.AnyAsync(p => p.Id == productId);
            if (!exists)
                throw new KeyNotFoundException("Product not found");
        }
        if(categoryId != null)
        {
            var exists = await _context.Categories.AnyAsync(c => c.Id == categoryId);
            if (!exists)
                throw new KeyNotFoundException("Category not found");
        }
        return ;
    }
}