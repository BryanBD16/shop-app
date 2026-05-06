export interface AdminProductDetailsDto {
  id: number;
  name: string;
  originalPrice: number;
  discountedPrice?: number | null;
  imagePath: string;
  description: string;
  stockQuantity: number;
  isPublished: boolean;
  categoryId: number;
}