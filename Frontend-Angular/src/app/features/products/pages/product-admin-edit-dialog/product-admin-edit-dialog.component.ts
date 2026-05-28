import { AfterViewInit, Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { CategoryService } from '../../../../core/services/category.service';
import { ProductService } from '../../../../core/services/product.service';
import { CategoryDto } from '../../../../shared/models/categories/category.dto.model';
import { AdminProductDetailsDto } from '../../../../shared/models/products/admin-product-details.dto.model';

@Component({
  selector: 'app-product-admin-edit-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatSlideToggleModule],
  templateUrl: './product-admin-edit-dialog.component.html',
  styleUrl: './product-admin-edit-dialog.component.scss'
})
export class ProductAdminEditDialogComponent implements AfterViewInit {

  name = '';
  price: number | null = null;
  description = '';
  stockQuantity: number | null = null;
  isPublished = true;
  categoryId: number | null = null;
  imagePath = '';
  error?: string;
  categories: CategoryDto[] = [];
  imageGallery: string[] = [];
  @ViewChild('dialogTop') dialogTop?: ElementRef<HTMLDivElement>;

  constructor(
    public dialogRef: MatDialogRef<ProductAdminEditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AdminProductDetailsDto & { error?: string },
    private categoryService: CategoryService,
    private productService: ProductService
  ) {
    this.name = data.name;
    this.price = data.originalPrice;
    this.description = data.description ?? '';
    this.stockQuantity = data.stockQuantity;
    this.isPublished = data.isPublished;
    this.categoryId = data.categoryId;
    this.imagePath = data.imagePath;
    this.error = data.error;

    this.categoryService.getCategories().subscribe({ next: (categories) => this.categories = categories, error: (error) => console.error(error) });
    this.loadImages();
  }

  ngAfterViewInit(): void {
    if (this.error) {
      this.scrollToTop();
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    const payload = {
      name: this.name.trim(),
      price: Number(this.price),
      imagePath: this.imagePath,
      description: this.description.trim(),
      stockQuantity: Number(this.stockQuantity),
      isPublished: this.isPublished,
      categoryId: this.categoryId
    };

    this.dialogRef.close(payload);
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

  private loadImages(): void {
    this.productService.getProductImages().subscribe({
      next: (images) => this.imageGallery = images,
      error: (error) => console.error('Error loading product images:', error)
    });
  }

  private scrollToTop(): void {
    this.dialogTop?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }
}