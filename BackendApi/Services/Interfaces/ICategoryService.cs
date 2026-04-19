using BackendApi.DTOs;
using BackendApi.Dtos.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendApi.Services.Interfaces;

public interface ICategoryService
{
    Task<int> CreateCategoryAsync(AdminCategoryCreateDto dto);
    Task<bool> UpdateCategoryAsync(int id, AdminCategoryUpdateDto dto);
    Task<bool> DeleteCategoryAsync(int id);
    Task<List<CategoryDto>> GetCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
}
