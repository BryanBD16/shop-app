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
    path: 'admin/products',
    loadComponent: () =>
      import('./features/products/pages/product-admin-list/product-admin-list.component')
        .then(m => m.ProductAdminListComponent)
  },
  {
    path: 'admin/products/create',
    loadComponent: () =>
      import('./features/products/pages/product-create/product-create.component')
        .then(m => m.ProductCreateComponent)
  },
  {
    path: 'admin/products/:id',
    loadComponent: () =>
      import('./features/products/pages/product-admin-detail/product-admin-detail.component')
        .then(m => m.ProductAdminDetailComponent)
  },
  {
    path: 'admin/discounts',
    loadComponent: () =>
      import('./features/discounts/pages/discount-list/discount-list.component')
        .then(m => m.DiscountListComponent)
  },
  {
    path: 'admin/discounts/create',
    loadComponent: () =>
      import('./features/discounts/pages/discount-create/discount-create.component')
        .then(m => m.DiscountCreateComponent)
  },
  {
    path: 'admin/categories',
    loadComponent: () =>
      import('./features/categories/pages/category-list/category-list.component')
        .then(m => m.CategoryListComponent)
  },
  {
    path: 'admin/categories/create',
    loadComponent: () =>
      import('./features/categories/pages/category-create/category-create.component')
        .then(m => m.CategoryCreateComponent)
  }
];