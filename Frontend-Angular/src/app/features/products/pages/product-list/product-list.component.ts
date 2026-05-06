import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import {MatButtonModule} from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { MatCardModule } from '@angular/material/card';
import { Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ProductService } from '../../../../core/services/product.service';
import { CategoryService } from '../../../../core/services/category.service';
import { LoadingComponent } from '../../../../shared/components/loading/loading.component';


@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [FormsModule, CommonModule, MatButtonModule, MatCardModule, MatCard, MatInputModule, MatSelectModule, MatFormFieldModule, LoadingComponent],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss'
})
export class ProductListComponent implements OnInit{
  isLoading = false;
  isLoadingCategories = false;
  currentPage = 1;
  pageSize = 12;
  currentSearch = '';
  currentCategoryId = '';

  products: any[] = [];
  categories: any[] = [];

  totalPages = 1;


  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private router: Router
  ) {}

  goToProduct(id: number) {
    this.router.navigate(['/products', id]);
  }

  ngOnInit() {
    this.fetchCategories();
    this.fetchProducts();
  }

  fetchProducts() {
    this.isLoading = true;

    this.productService
      .getProducts(this.currentPage, this.currentSearch, this.currentCategoryId)
      .subscribe({
        next: (data) => {
          this.products = data.items;
          this.totalPages = data.totalPages;
          this.currentPage = data.currentPage;
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
        }
      });
  }

  fetchCategories() {
    this.isLoadingCategories = true;

    this.categoryService.getCategories().subscribe({
      next: (data) => {
        this.categories = data;
        this.isLoadingCategories = false;
      },
      error: () => {
        this.isLoadingCategories = false;
      }
    });
  }

  onSearchChange() {
    this.currentPage = 1;
    this.fetchProducts();
  }

  onCategoryChange() {
    this.currentPage = 1;
    this.fetchProducts();
  }

  prevPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.fetchProducts();
    }
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.fetchProducts();
    }
  }

  renderPrice(original: number, discounted?: number) {
    return discounted ?? original;
  }
}
