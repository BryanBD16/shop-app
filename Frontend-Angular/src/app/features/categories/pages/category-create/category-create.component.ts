import { Component, ElementRef, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';
import { CategoryService } from '../../../../core/services/category.service';
import { AdminCategoryCreateDto } from '../../../../shared/models/categories/admin-category-create.dto.model';

@Component({
  selector: 'app-category-create',
  standalone: true,
  imports: [CommonModule, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './category-create.component.html',
  styleUrl: './category-create.component.scss'
})
export class CategoryCreateComponent {

  private categoryService = inject(CategoryService);
  private router = inject(Router);

  @ViewChild('formTop') formTop?: ElementRef<HTMLDivElement>;

  name: string = '';
  error: string | null = null;
  isSubmitting = false;

  onSubmit(): void {
    this.error = null;
    this.isSubmitting = true;

    if (!this.name.trim()) {
      this.fail('Category name is required');
      return;
    }

    const dto: AdminCategoryCreateDto = { name: this.name };

    this.categoryService.createCategory(dto).subscribe({
      next: (result) => {
        this.isSubmitting = false;
        this.router.navigate(['/admin/categories']);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.scrollToTop();

        // Extraire le premier message d'erreur de l'API
        if (err.error?.detail) {
          this.error = err.error.detail;
        } else if (err.error?.errors) {
          // Si c'est un objet avec plusieurs erreurs (ValidationProblemDetails), prendre la première
          const firstError = Object.values(err.error.errors)[0];
          if (Array.isArray(firstError) && firstError.length > 0) {
            this.error = firstError[0];
          } else {
            this.error = 'Failed to create category';
          }
        } else {
          this.error = 'Failed to create category';
        }

        console.error('Error creating category:', err);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/categories']);
  }

  private fail(message: string): void {
    this.error = message;
    this.isSubmitting = false;
    this.scrollToTop();
  }

  private scrollToTop(): void {
    this.formTop?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }
}
