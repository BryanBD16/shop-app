import { CommonModule } from '@angular/common';
import { Component, ElementRef, ViewChild, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { catchError, forkJoin, of } from 'rxjs';
import { CategoryService } from '../../../../core/services/category.service';
import { ProductService } from '../../../../core/services/product.service';
import { AdminProductCreateDto } from '../../../../shared/models/products/admin-product-create.dto.model';
import { CategoryDto } from '../../../../shared/models/categories/category.dto.model';

@Component({
  selector: 'app-product-create',
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  templateUrl: './product-create.component.html',
  styleUrl: './product-create.component.scss'
})
export class ProductCreateComponent {

  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private router = inject(Router);

  name = '';
  price: number | null = null;
  description = '';
  stockQuantity: number | null = null;
  isPublished = true;
  categoryId: number | null = null;
  imagePath = '';
  error: string | null = null;
  isSubmitting = false;

  categories: CategoryDto[] = [];
  imageGallery: string[] = [];
  @ViewChild('formTop') formTop?: ElementRef<HTMLDivElement>;

  constructor() {
    this.loadInitialData();
  }

  onSubmit(): void {
    this.error = null;
    this.isSubmitting = true;

    if (!this.name.trim()) {
      this.fail('Product name is required');
      return;
    }

    if (this.price == null || Number.isNaN(Number(this.price))) {
      this.fail('Product price is required');
      return;
    }
    if (this.price < 0) {
      this.fail('Product price cannot be negative');
      return;
    }

    if (this.stockQuantity == null || Number.isNaN(Number(this.stockQuantity))) {
      this.fail('Stock quantity is required');
      return;
    }

    if (this.stockQuantity < 0) {
      this.fail('Stock quantity cannot be negative');
      return;
    }

    if (!this.imagePath) {
      this.fail('Please select an image');
      return;
    }

    if (this.categoryId == null) {
      this.fail('Category is required');
      return;
    }

    const dto: AdminProductCreateDto = {
      name: this.name.trim(),
      price: Number(this.price),
      imagePath: this.imagePath,
      description: this.description.trim(),
      stockQuantity: Number(this.stockQuantity),
      isPublished: this.isPublished,
      categoryId: this.categoryId
    };

    this.productService.createProduct(dto).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/admin/products']);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.error = this.extractErrorMessage(err, 'Failed to create product');
        console.error('Error creating product:', err);
        this.scrollToTop();
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/products']);
  }

  selectImage(imagePath: string): void {
    this.imagePath = imagePath;
  }

  imageUrl(imagePath: string): string {
    return `http://localhost:5000${imagePath}`;
  }

  isSelectedImage(imagePath: string): boolean {
    return this.imagePath === imagePath;
  }

  private fail(message: string): void {
    this.error = message;
    this.isSubmitting = false;
    this.scrollToTop();
  }

  private scrollToTop(): void {
    this.formTop?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

  private loadInitialData(): void {
    forkJoin({
      categories: this.categoryService.getCategories().pipe(catchError(() => of([] as CategoryDto[]))),
      images: this.productService.getProductImages().pipe(catchError(() => of([] as string[])))
    }).subscribe({
      next: ({ categories, images }) => {
        this.categories = categories;
        this.imageGallery = images;

        if (!this.imagePath && this.imageGallery.length > 0) {
          this.imagePath = this.imageGallery[0];
        }
      },
      error: (error) => console.error('Error loading product create data:', error)
    });
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