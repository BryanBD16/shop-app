using BackendApi.DTOs;
using BackendApi.Dtos.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendApi.Services.Interfaces;

public interface IProductService
{
    Task<PagedResultDto<ProductListItemDto>> GetPublishedProductsAsync(int page, string search, int? categoryId = null);
    Task<ProductDetailsDto?> GetPublishedProductByIdAsync(int id);

    Task<PagedResultDto<AdminProductListItemDto>> GetAdminProductsAsync(int page, string search, int? categoryId = null);
    Task<AdminProductDetailsDto?> GetAdminProductByIdAsync(int id);

    Task<int> CreateProductAsync(AdminProductCreateDto dto);
    Task<bool> UpdateProductAsync(int id, AdminProductUpdateDto dto);
    Task<bool> DeleteProductAsync(int id);

    List<string> GetProductImages();
}
