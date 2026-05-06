export interface AdminProductListItemDto {
  id: number;
  name: string;
  originalPrice: number;
  discountedPrice?: number | null;
  imagePath: string;
  stockQuantity: number;
  isPublished: boolean;
  categoryId: number;
}