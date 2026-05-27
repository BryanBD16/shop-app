import { Component, ElementRef, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { catchError, debounceTime, distinctUntilChanged, map, of, switchMap } from 'rxjs';
import { CategoryService } from '../../../../core/services/category.service';
import { DiscountService } from '../../../../core/services/discount.service';
import { ProductService } from '../../../../core/services/product.service';
import { AdminDiscountCreateDto } from '../../../../shared/models/discounts/admin-discount-create.dto.model';
import { CategoryDto } from '../../../../shared/models/categories/category.dto.model';
import { AdminProductListItemDto } from '../../../../shared/models/products/admin-product-list-item.dto.model';
import { localDateTimeToUtcIso } from '../../../../shared/utils/discount-date.utils';

@Component({
  selector: 'app-discount-create',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MatAutocompleteModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  templateUrl: './discount-create.component.html',
  styleUrl: './discount-create.component.scss'
})
export class DiscountCreateComponent {

  private discountService = inject(DiscountService);
  private categoryService = inject(CategoryService);
  private productService = inject(ProductService);
  private router = inject(Router);

  @ViewChild('formTop') formTop?: ElementRef<HTMLDivElement>;

  title = '';
  percentage: number | null = null;
  startDate = '';
  endDate = '';
  productId: number | null = null;
  categoryId: number | null = null;
  error: string | null = null;
  isSubmitting = false;
  categories: CategoryDto[] = [];
  filteredProducts: AdminProductListItemDto[] = [];
  productControl = new FormControl<AdminProductListItemDto | string | null>('');

  constructor() {
    this.categoryService.getCategories().subscribe({
      next: (categories) => this.categories = categories,
      error: (error) => console.error('Error loading categories:', error)
    });

    this.bindProductSearch();
  }

  onSubmit(): void {
    this.error = null;
    this.isSubmitting = true;
    this.syncSelectedProduct();

    if (!this.title.trim()) {
      this.fail('Discount title is required');
      return;
    }

    if (this.percentage == null || Number.isNaN(Number(this.percentage))) {
      this.fail('Discount percentage is required');
      return;
    }

    if (this.percentage < 0 || this.percentage > 100) {
      this.fail('Discount percentage must be between 0 and 100');
      return;
    }

    if (!this.startDate) {
      this.fail('Start date is required');
      return;
    }

    const dto: AdminDiscountCreateDto = {
      title: this.title.trim(),
      percentage: Number(this.percentage),
      startDate: localDateTimeToUtcIso(this.startDate),
      endDate: this.endDate ? localDateTimeToUtcIso(this.endDate) : null,
      productId: this.productId,
      categoryId: this.categoryId
    };

    this.discountService.createDiscount(dto).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/admin/discounts']);
      },
      error: (error) => {
        this.isSubmitting = false;
        this.error = this.extractErrorMessage(error, 'Failed to create discount');
        console.error('Error creating discount:', error);
        this.scrollToTop();
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/discounts']);
  }

  onProductSelected(product: AdminProductListItemDto): void {
    this.productId = product.id;
    this.productControl.setValue(product, { emitEvent: false });
  }

  displayProduct = (value: AdminProductListItemDto | string | null): string => {
    if (typeof value === 'string') {
      return value;
    }

    return value?.name ?? '';
  };

  private bindProductSearch(): void {
    this.productControl.valueChanges.pipe(
      map((value) => typeof value === 'string' ? value.trim() : value?.name?.trim() ?? ''),
      debounceTime(250),
      distinctUntilChanged(),
      switchMap((search) => {
        if (!search) {
          return of([] as AdminProductListItemDto[]);
        }

        return this.productService.getAdminProducts(1, search).pipe(
          map((result) => result.items.slice(0, 12)),
          catchError(() => of([] as AdminProductListItemDto[]))
        );
      })
    ).subscribe((products) => {
      this.filteredProducts = products;
    });
  }

  private syncSelectedProduct(): void {
    const value = this.productControl.value;

    if (value && typeof value !== 'string') {
      this.productId = value.id;
      return;
    }

    const search = typeof value === 'string' ? value.trim() : '';
    if (!search) {
      this.productId = null;
      return;
    }

    const match = this.filteredProducts.find((product) => product.name.toLowerCase() === search.toLowerCase());
    this.productId = match?.id ?? null;
    if (match) {
      this.productControl.setValue(match, { emitEvent: false });
    }
  }

  private fail(message: string): void {
    this.error = message;
    this.isSubmitting = false;
    this.scrollToTop();
  }

  private scrollToTop(): void {
    this.formTop?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

  private extractErrorMessage(error: any, fallback: string): string {
    if (error.error?.detail) {
      return error.error.detail;
    }

    if (error.error?.errors) {
      const firstError = Object.values(error.error.errors)[0];
      if (Array.isArray(firstError) && firstError.length > 0) {
        return firstError[0];
      }
    }

    return fallback;
  }
}