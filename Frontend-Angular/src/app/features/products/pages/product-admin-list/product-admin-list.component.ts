import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { Router } from '@angular/router';
import { CategoryService } from '../../../../core/services/category.service';
import { ProductService } from '../../../../core/services/product.service';
import { CategoryDto } from '../../../../shared/models/categories/category.dto.model';
import { AdminProductListItemDto } from '../../../../shared/models/products/admin-product-list-item.dto.model';

@Component({
  selector: 'app-product-admin-list',
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatIconModule, MatInputModule, MatSelectModule],
  templateUrl: './product-admin-list.component.html',
  styleUrl: './product-admin-list.component.scss'
})
export class ProductAdminListComponent implements OnInit {

  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private router = inject(Router);

  products: AdminProductListItemDto[] = [];
  categories: CategoryDto[] = [];
  currentPage = 1;
  pageSize = 12;
  totalPages = 1;
  currentSearch = '';
  currentCategoryId = '';
  isLoading = false;

  ngOnInit(): void {
    this.fetchCategories();
    this.fetchProducts();
  }

  openDetail(product: AdminProductListItemDto): void {
    this.router.navigate(['/admin/products', product.id]);
  }

  fetchProducts(): void {
    this.isLoading = true;
    this.productService.getAdminProducts(this.currentPage, this.currentSearch, this.currentCategoryId || undefined).subscribe({
      next: (data) => {
        this.products = data.items;
        this.totalPages = data.totalPages;
        this.currentPage = data.currentPage;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading admin products:', error);
        this.isLoading = false;
      }
    });
  }

  fetchCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: (data) => {
        this.categories = data;
      },
      error: (error) => console.error('Error loading categories:', error)
    });
  }

  onSearchChange(): void {
    this.currentPage = 1;
    this.fetchProducts();
  }

  onCategoryChange(): void {
    this.currentPage = 1;
    this.fetchProducts();
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.fetchProducts();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.fetchProducts();
    }
  }

  getCategoryName(categoryId: number): string {
    const category = this.categories.find((item) => item.id === categoryId);
    return category?.name ?? `Category #${categoryId}`;
  }

  getImageUrl(imagePath: string): string {
    return `http://localhost:5000${imagePath}`;
  }

  renderPrice(original: number, discounted?: number | null): number {
    return discounted ?? original;
  }

  isPublishedLabel(product: AdminProductListItemDto): string {
    return product.isPublished ? 'Published' : 'Unpublished';
  }

}