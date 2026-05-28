import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { ProductService } from '../../../../core/services/product.service';
import { CategoryService } from '../../../../core/services/category.service';
import { AdminProductDetailsDto } from '../../../../shared/models/products/admin-product-details.dto.model';
import { CategoryDto } from '../../../../shared/models/categories/category.dto.model';
import { ProductAdminEditDialogComponent } from '../product-admin-edit-dialog/product-admin-edit-dialog.component';

@Component({
  selector: 'app-product-admin-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, MatButtonModule, MatCardModule],
  templateUrl: './product-admin-detail.component.html',
  styleUrl: './product-admin-detail.component.scss'
})
export class ProductAdminDetailComponent implements OnInit {

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private dialog = inject(MatDialog);

  product: AdminProductDetailsDto | null = null;
  error: string | null = null;
  categoryName: string | null = null;

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const id = Number(params.get('id'));

      if (!id) {
        this.error = 'Invalid product id';
        return;
      }

      this.loadProduct(id);
    });
  }

  openEditDialog(): void {
    if (!this.product) {
      return;
    }

    const dialogRef = this.dialog.open(ProductAdminEditDialogComponent, {
      width: '900px',
      data: { ...this.product }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.updateProduct(this.product!.id, result);
      }
    });
  }

  onBack(): void {
    this.router.navigate(['/admin/products']);
  }

  getImageUrl(imagePath: string): string {
    return `http://localhost:5000${imagePath}`;
  }

  renderPrice(original: number, discounted?: number | null): number {
    return discounted ?? original;
  }

  private loadProduct(id: number): void {
    this.error = null;

    this.productService.getAdminProduct(id).subscribe({
      next: (data) => {
        this.product = data;
        this.loadCategoryName(data.categoryId);
      },
      error: (err) => {
        console.error(err);

        if (err.status === 404) {
          this.error = 'Product not found';
        } else {
          this.error = 'Unexpected error occurred';
        }
      }
    });
  }

  private loadCategoryName(categoryId: number): void {
    this.categoryService.getCategory(categoryId).subscribe({
      next: (category: CategoryDto) => {
        this.categoryName = category.name;
      },
      error: () => {
        this.categoryName = `Category #${categoryId}`;
      }
    });
  }

  private updateProduct(id: number, data: any): void {
    this.productService.updateProduct(id, data).subscribe({
      next: () => this.loadProduct(id),
      error: (error) => {
        const errorMessage = this.extractErrorMessage(error, 'Failed to update product');

        if (this.product) {
          const dialogRef = this.dialog.open(ProductAdminEditDialogComponent, {
            width: '900px',
            data: { ...this.product, ...data, error: errorMessage }
          });

          dialogRef.afterClosed().subscribe((result) => {
            if (result) {
              this.updateProduct(id, result);
            }
          });
        }
      }
    });
  }

  private extractErrorMessage(error: any, fallback: string): string {
    if (error.error?.detail) {
      return error.error.detail;
    }

    if (error.error?.errors) {
      const first = Object.values(error.error.errors)[0];
      if (Array.isArray(first) && first.length > 0) {
        return first[0];
      }
    }

    return fallback;
  }
}