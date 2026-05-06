import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminProductCreateDto } from '../../shared/models/products/admin-product-create.dto.model';
import { AdminProductDetailsDto } from '../../shared/models/products/admin-product-details.dto.model';
import { AdminProductListItemDto } from '../../shared/models/products/admin-product-list-item.dto.model';
import { AdminProductUpdateDto } from '../../shared/models/products/admin-product-update.dto.model';
import { ProductListItemDto } from '../../shared/models/products/product.list.item.dto.model';
import { ProductDetailsDto } from '../../shared/models/products/product-details.dto.model';
import { PagedResultDto } from '../../shared/models/products/paged.result.dto.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private apiUrl = 'http://localhost:5000/api/products';
  private adminApiUrl = 'http://localhost:5000/api/admin/products';
  private adminImageApiUrl = 'http://localhost:5000/api/admin/product-images';

  constructor(private http: HttpClient) {}

  getProducts(
    page: number,
    search: string,
    categoryId?: string
  ): Observable<PagedResultDto<ProductListItemDto>> {
    let url = `${this.apiUrl}?page=${page}&search=${search}`;

    if (categoryId) {
      url += `&categoryId=${categoryId}`;
    }

    return this.http.get<PagedResultDto<ProductListItemDto>>(url);
  }

  getProduct(id: number): Observable<ProductDetailsDto> {
    return this.http.get<ProductDetailsDto>(`${this.apiUrl}/${id}`);
  }

  getAdminProducts(
    page: number,
    search: string,
    categoryId?: string
  ): Observable<PagedResultDto<AdminProductListItemDto>> {
    let url = `${this.adminApiUrl}?page=${page}&search=${search}`;

    if (categoryId) {
      url += `&categoryId=${categoryId}`;
    }

    return this.http.get<PagedResultDto<AdminProductListItemDto>>(url);
  }

  getAdminProduct(id: number): Observable<AdminProductDetailsDto> {
    return this.http.get<AdminProductDetailsDto>(`${this.adminApiUrl}/${id}`);
  }

  createProduct(dto: AdminProductCreateDto): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.adminApiUrl, dto);
  }

  updateProduct(id: number, dto: AdminProductUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.adminApiUrl}/${id}`, dto);
  }

  getProductImages(): Observable<string[]> {
    return this.http.get<string[]>(this.adminImageApiUrl);
  }
}