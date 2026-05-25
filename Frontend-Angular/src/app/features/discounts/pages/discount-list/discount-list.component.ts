import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { DiscountService } from '../../../../core/services/discount.service';
import { CategoryService } from '../../../../core/services/category.service';
import { ProductService } from '../../../../core/services/product.service';
import { DiscountDto } from '../../../../shared/models/discounts/discount.dto.model';
import { DiscountEditDialogComponent } from '../discount-edit-dialog/discount-edit-dialog.component';
import { utcIsoToLocalDateTimeInput } from '../../../../shared/utils/discount-date.utils';

@Component({
  selector: 'app-discount-list',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule],
  templateUrl: './discount-list.component.html',
  styleUrl: './discount-list.component.scss'
})
export class DiscountListComponent implements OnInit {

  private discountService = inject(DiscountService);
  private dialog = inject(MatDialog);
  private categoryService = inject(CategoryService);
  private productService = inject(ProductService);

  discounts: DiscountDto[] = [];
  displayedColumns: string[] = ['title', 'percentage', 'startDate', 'endDate', 'scope', 'actions'];
  isLoading = false;
  private categoryNames = new Map<number, string>();
  private productNames = new Map<number, string>();

  ngOnInit(): void {
    this.loadDiscounts();
  }

  loadDiscounts(): void {
    this.isLoading = true;
    this.discountService.getDiscounts().subscribe({
      next: (data) => {
        this.discounts = data;
        this.resolveScopeLabels(data);
      },
      error: (err) => {
        console.error('Error loading discounts:', err);
        this.isLoading = false;
      }
    });
  }

  private resolveScopeLabels(discounts: DiscountDto[]): void {
    const productIds = [...new Set(discounts.map(discount => discount.productId).filter((id): id is number => id != null))];
    const categoryIds = [...new Set(discounts.map(discount => discount.categoryId).filter((id): id is number => id != null))];

    const requests = [
      ...productIds.map((id) =>
        this.productService.getAdminProduct(id).pipe(
          map((product) => ({ kind: 'product' as const, id, name: product.name })),
          catchError(() => of(null))
        )
      ),
      ...categoryIds.map((id) =>
        this.categoryService.getCategory(id).pipe(
          map((category) => ({ kind: 'category' as const, id, name: category.name })),
          catchError(() => of(null))
        )
      )
    ];

    if (requests.length === 0) {
      this.isLoading = false;
      return;
    }

    forkJoin(requests).subscribe({
      next: (results) => {
        results.forEach((result) => {
          if (!result) {
            return;
          }

          if (result.kind === 'product') {
            this.productNames.set(result.id, result.name);
          } else {
            this.categoryNames.set(result.id, result.name);
          }
        });

        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error resolving discount labels:', err);
        this.isLoading = false;
      }
    });
  }

  openEditDialog(discount: DiscountDto): void {
    const dialogRef = this.dialog.open(DiscountEditDialogComponent, {
      width: '500px',
      data: { ...discount }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.updateDiscount(discount.id, result, discount);
      }
    });
  }

  updateDiscount(id: number, data: any, discount?: DiscountDto): void {
    this.discountService.updateDiscount(id, data).subscribe({
      next: () => {
        this.loadDiscounts();
      },
      error: (err) => {
        console.error('Error updating discount:', err);

        let errorMessage = 'Failed to update discount';
        if (err.error?.detail) {
          errorMessage = err.error.detail;
        } else if (err.error?.errors) {
          const first = Object.values(err.error.errors)[0];
          if (Array.isArray(first) && first.length > 0) {
            errorMessage = first[0];
          }
        }

        if (discount) {
          const dialogRef = this.dialog.open(DiscountEditDialogComponent, {
            width: '500px',
            data: { ...discount, ...data, error: errorMessage }
          });

          dialogRef.afterClosed().subscribe((res) => {
            if (res) {
              this.updateDiscount(discount.id, res, discount);
            }
          });
        }
      }
    });
  }

  deleteDiscount(id: number): void {
    if (!confirm('Are you sure you want to delete this discount?')) return;

    this.discountService.deleteDiscount(id).subscribe({
      next: () => this.loadDiscounts(),
      error: (err) => console.error('Error deleting discount:', err)
    });
  }

  getScopeLabel(discount: DiscountDto): string {
    if (discount.productId != null) {
      return this.productNames.get(discount.productId) ?? `Product #${discount.productId}`;
    }

    if (discount.categoryId != null) {
      return this.categoryNames.get(discount.categoryId) ?? `Category #${discount.categoryId}`;
    }

    return '-';
  }

  formatDiscountDate(value?: string | null): string {
    const localDateTime = utcIsoToLocalDateTimeInput(value);

    if (!localDateTime) {
      return '-';
    }

    return new Date(localDateTime).toLocaleString();
  }
}

