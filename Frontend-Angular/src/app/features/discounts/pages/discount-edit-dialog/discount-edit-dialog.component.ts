import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormControl } from '@angular/forms';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { catchError, debounceTime, distinctUntilChanged, map, of, switchMap } from 'rxjs';
import { DiscountDto } from '../../../../shared/models/discounts/discount.dto.model';
import { CategoryService } from '../../../../core/services/category.service';
import { ProductService } from '../../../../core/services/product.service';
import { CategoryDto } from '../../../../shared/models/categories/category.dto.model';
import { AdminProductListItemDto } from '../../../../shared/models/products/admin-product-list-item.dto.model';
import { AdminProductDetailsDto } from '../../../../shared/models/products/admin-product-details.dto.model';
import { localDateTimeToUtcIso, utcIsoToLocalDateTimeInput } from '../../../../shared/utils/discount-date.utils';

@Component({
  selector: 'app-discount-edit-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MatDialogModule, MatAutocompleteModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSelectModule, MatIconModule],
  templateUrl: './discount-edit-dialog.component.html',
  styleUrl: './discount-edit-dialog.component.scss'
})
export class DiscountEditDialogComponent {
  title: string = '';
  percentage: number | null = null;
  startDate: string = '';
  endDate?: string | null = null;
  productId?: number | null = null;
  categoryId?: number | null = null;
  error?: string;
  categories: CategoryDto[] = [];
  products: AdminProductListItemDto[] = [];
  filteredProducts: AdminProductListItemDto[] = [];
  productControl = new FormControl<AdminProductListItemDto | string | null>('');

  constructor(
    public dialogRef: MatDialogRef<DiscountEditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DiscountDto & { error?: string },
    private categoryService: CategoryService,
    private productService: ProductService
  ) {
    this.title = data.title;
    this.percentage = data.percentage;
    this.startDate = utcIsoToLocalDateTimeInput(data.startDate);
    this.endDate = utcIsoToLocalDateTimeInput(data.endDate);
    this.productId = data.productId ?? null;
    this.categoryId = data.categoryId ?? null;
    this.error = data.error;
    this.categoryService.getCategories().subscribe({ next: cats => this.categories = cats, error: (e) => console.error(e) });
    this.bindProductSearch();

    if (this.productId != null) {
      this.loadProductById(this.productId);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    this.syncSelectedProduct();

    const payload: any = {
      title: this.title,
      percentage: Number(this.percentage),
      startDate: localDateTimeToUtcIso(this.startDate),
      endDate: this.endDate ? localDateTimeToUtcIso(this.endDate) : null,
      productId: this.productId || null,
      categoryId: this.categoryId || null
    };

    this.dialogRef.close(payload);
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

  private loadProductById(id: number): void {
    this.productService.getAdminProduct(id).subscribe({
      next: (product: AdminProductDetailsDto) => {
        const selectedProduct: AdminProductListItemDto = {
          id: product.id,
          name: product.name,
          originalPrice: product.originalPrice,
          discountedPrice: product.discountedPrice ?? null,
          imagePath: product.imagePath,
          stockQuantity: product.stockQuantity,
          isPublished: product.isPublished,
          categoryId: product.categoryId
        };

        this.products = [selectedProduct];
        this.filteredProducts = [selectedProduct];
        this.productControl.setValue(selectedProduct, { emitEvent: false });
      },
      error: (error) => console.error('Error loading selected product', error)
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

}
