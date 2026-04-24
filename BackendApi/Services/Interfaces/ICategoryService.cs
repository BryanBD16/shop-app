using BackendApi.DTOs;
using BackendApi.Dtos.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendApi.Services.Interfaces;

public interface ICategoryService
{
    Task<int> CreateCategoryAsync(AdminCategoryCreateDto dto);
    Task UpdateCategoryAsync(int id, AdminCategoryUpdateDto dto);
    Task DeleteCategoryAsync(int id);
    Task<List<CategoryDto>> GetCategoriesAsync();
    Task<CategoryDto> GetCategoryByIdAsync(int id);
}
