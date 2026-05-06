import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminDiscountCreateDto } from '../../shared/models/discounts/admin-discount-create.dto.model';
import { AdminDiscountUpdateDto } from '../../shared/models/discounts/admin-discount-update.dto.model';
import { DiscountDto } from '../../shared/models/discounts/discount.dto.model';

@Injectable({ providedIn: 'root' })
export class DiscountService {
  private apiUrl = 'http://localhost:5000/api/discounts';
  private adminApiUrl = 'http://localhost:5000/api/admin/discounts';

  constructor(private http: HttpClient) {}

  getDiscounts(startDate?: string, endDate?: string): Observable<DiscountDto[]> {
    const params = new URLSearchParams();

    if (startDate) {
      params.set('startDate', startDate);
    }

    if (endDate) {
      params.set('endDate', endDate);
    }

    const query = params.toString();

    return this.http.get<DiscountDto[]>(query ? `${this.apiUrl}?${query}` : this.apiUrl);
  }

  getDiscount(id: number): Observable<DiscountDto> {
    return this.http.get<DiscountDto>(`${this.apiUrl}/${id}`);
  }

  createDiscount(dto: AdminDiscountCreateDto): Observable<number> {
    return this.http.post<number>(this.adminApiUrl, dto);
  }

  updateDiscount(id: number, dto: AdminDiscountUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  deleteDiscount(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}