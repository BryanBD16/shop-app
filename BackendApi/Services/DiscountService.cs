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
        // TODO
        throw new NotImplementedException();
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
   
}