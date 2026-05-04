import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import {MatButtonModule} from '@angular/material/button';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [FormsModule, CommonModule, MatButtonModule],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss'
})
export class ProductListComponent implements OnInit{
currentPage = 1;
  pageSize = 12;
  currentSearch = '';
  currentCategoryId = '';

  products: any[] = [];
  categories: any[] = [];

  totalPages = 1;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.fetchCategories();
    this.fetchProducts();
  }

  fetchProducts() {
    let url = `http://localhost:5000/api/products?page=${this.currentPage}&search=${this.currentSearch}`;

    if (this.currentCategoryId) {
      url += `&categoryId=${this.currentCategoryId}`;
    }

    this.http.get<any>(url).subscribe(data => {
      this.products = data.items;
      this.totalPages = data.totalPages;
      this.currentPage = data.currentPage;
    });
  }

  fetchCategories() {
    this.http.get<any[]>('http://localhost:5000/api/categories')
      .subscribe(data => this.categories = data);
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
