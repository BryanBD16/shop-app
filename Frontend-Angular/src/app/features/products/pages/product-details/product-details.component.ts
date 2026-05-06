import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../../../core/services/product.service';
import { ProductDetailsDto } from '../../../../shared/models/products/product-details.dto.model';
import { MatButtonModule } from '@angular/material/button';
import { LoadingComponent } from '../../../../shared/components/loading/loading.component';

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [CommonModule, RouterModule, MatButtonModule, LoadingComponent],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent implements OnInit {

  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);

  product: ProductDetailsDto | null = null;
  isLoading = true;
  error: string | null = null;

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));

      if (!id) {
        this.error = 'Invalid product id';
        this.isLoading = false;
        return;
      }

      this.loadProduct(id);
    });
  }

  private loadProduct(id: number): void {
    this.isLoading = true;
    this.error = null;

    this.productService.getProduct(id).subscribe({
      next: (data) => {
        this.product = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);

        if (err.status === 404) {
          this.error = 'Product not found';
        } else {
          this.error = 'Unexpected error occurred';
        }

        this.isLoading = false;
      }
    });
  }
}