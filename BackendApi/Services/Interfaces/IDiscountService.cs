using BackendApi.DTOs;
using BackendApi.Dtos.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace BackendApi.Services.Interfaces;

public interface IDiscountService
{
    Task<int> CreateDiscountAsync(AdminDiscountCreateDto dto);
    Task<bool> UpdateDiscountAsync(int id, AdminDiscountUpdateDto dto);
    Task<List<DiscountDto>> GetDiscountsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<DiscountDto> GetDiscountByIdAsync(int id);
}
