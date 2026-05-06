import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'products',
    pathMatch: 'full'
  },
  {
    path: 'products',
    loadComponent: () =>
      import('./features/products/pages/product-list/product-list.component')
        .then(m => m.ProductListComponent)
  },
  {
    path: 'products/:id',
    loadComponent: () =>
      import('./features/products/pages/product-details/product-details.component')
        .then(m => m.ProductDetailsComponent)
  },
  {
    path: 'admin/discounts',
    loadComponent: () =>
      import('./features/discounts/pages/discount-list/discount-list.component')
        .then(m => m.DiscountListComponent)
  },
  {
    path: 'admin/categories',
    loadComponent: () =>
      import('./features/categories/pages/category-list/category-list.component')
        .then(m => m.CategoryListComponent)
  }
];