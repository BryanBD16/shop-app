import { Component, Inject } from '@angular/core';
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
export class CategoryEditDialogComponent {

  name: string = '';
  error: string | undefined;

  constructor(
    public dialogRef: MatDialogRef<CategoryEditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CategoryEditDialogData
  ) {
    this.name = data.name;
    this.error = data.error;
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    if (this.name.trim()) {
      this.dialogRef.close({ name: this.name });
    }
  }
}
