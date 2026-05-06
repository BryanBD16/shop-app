export interface ProductListItemDto {
  id: number;
  name: string;
  originalPrice: number;
  discountedPrice?: number;
  imagePath: string;
  categoryId: number;
}