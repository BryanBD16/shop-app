using BackendApi.DTOs;
using BackendApi.Dtos.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendApi.Services.Interfaces;

public interface IDiscountService
{
    Task<int> CreateDiscountAsync(AdminDiscountCreateDto dto);
    Task<bool> UpdateDiscountAsync(int id, AdminDiscountUpdateDto dto);
    Task<List<DiscountDto>> GetDiscountsAsync();
    Task<DiscountDto?> GetDiscountByIdAsync(int id);
}
