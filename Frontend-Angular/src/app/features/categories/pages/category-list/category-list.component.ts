import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { CategoryService } from '../../../../core/services/category.service';
import { CategoryDto } from '../../../../shared/models/categories/category.dto.model';
import { CategoryEditDialogComponent } from '../category-edit-dialog/category-edit-dialog.component';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule],
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.scss'
})
export class CategoryListComponent implements OnInit {

  private categoryService = inject(CategoryService);
  private dialog = inject(MatDialog);

  categories: CategoryDto[] = [];
  displayedColumns: string[] = ['name', 'actions'];
  isLoading = false;

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.categoryService.getCategories().subscribe({
      next: (data) => {
        this.categories = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading categories:', err);
        this.isLoading = false;
      }
    });
  }

  openEditDialog(category: CategoryDto): void {
    const dialogRef = this.dialog.open(CategoryEditDialogComponent, {
      width: '400px',
      data: { ...category }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.updateCategory(category.id, result, category);
      }
    });
  }

  updateCategory(id: number, data: { name: string }, category?: CategoryDto): void {
    this.categoryService.updateCategory(id, { name: data.name }).subscribe({
      next: () => {
        this.loadCategories();
      },
      error: (err) => {
        console.error('Error updating category:', err);
        const errorMessage = err.error?.detail || 'Failed to update category';
        
        if (category) {
          const dialogRef = this.dialog.open(CategoryEditDialogComponent, {
            width: '400px',
            data: { ...category, name: data.name, error: errorMessage }
          });

          dialogRef.afterClosed().subscribe((result) => {
            if (result) {
              this.updateCategory(category.id, result, category);
            }
          });
        }
      }
    });
  }

  deleteCategory(id: number): void {
    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.deleteCategory(id).subscribe({
        next: () => {
          this.loadCategories();
        },
        error: (err) => {
          console.error('Error deleting category:', err);
        }
      });
    }
  }
}
