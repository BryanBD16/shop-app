export interface AdminProductUpdateDto {
  name: string;
  price: number;
  imagePath: string;
  description: string;
  stockQuantity: number;
  isPublished: boolean;
  categoryId: number;
}