import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminCategoryCreateDto } from '../../shared/models/categories/admin-category-create.dto.model';
import { AdminCategoryUpdateDto } from '../../shared/models/categories/admin-category-update.dto.model';
import { CategoryDto } from '../../shared/models/categories/category.dto.model';
import { delay } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })

export class CategoryService {
  private apiUrl = 'http://localhost:5000/api/categories';
  private adminApiUrl = 'http://localhost:5000/api/admin/categories';

  constructor(private http: HttpClient) {}

  getCategories(): Observable<CategoryDto[]> {
    return this.http.get<CategoryDto[]>(this.apiUrl);
  }

  getCategory(id: number): Observable<CategoryDto> {
    return this.http.get<CategoryDto>(`${this.apiUrl}/${id}`);
  }

  createCategory(dto: AdminCategoryCreateDto): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.adminApiUrl, dto);
  }

  updateCategory(id: number, dto: AdminCategoryUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.adminApiUrl}/${id}`, dto);
  }

  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.adminApiUrl}/${id}`);
  }
}