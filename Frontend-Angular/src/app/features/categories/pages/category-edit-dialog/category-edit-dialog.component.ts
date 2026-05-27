import { AfterViewInit, Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CategoryDto } from '../../../../shared/models/categories/category.dto.model';

export interface CategoryEditDialogData extends CategoryDto {
  error?: string;
}

@Component({
  selector: 'app-category-edit-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './category-edit-dialog.component.html',
  styleUrl: './category-edit-dialog.component.scss'
})
export class CategoryEditDialogComponent implements AfterViewInit {

  name: string = '';
  error: string | undefined;
  @ViewChild('dialogTop') dialogTop?: ElementRef<HTMLDivElement>;

  constructor(
    public dialogRef: MatDialogRef<CategoryEditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CategoryEditDialogData
  ) {
    this.name = data.name;
    this.error = data.error;
  }

  ngAfterViewInit(): void {
    if (this.error) {
      this.scrollToTop();
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    if (this.name.trim()) {
      this.dialogRef.close({ name: this.name });
      return;
    }

    this.error = 'Category name is required';
    this.scrollToTop();
  }

  private scrollToTop(): void {
    this.dialogTop?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }
}
