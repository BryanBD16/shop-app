export interface ProductDetailsDto {
  id: number;
  name: string;
  originalPrice: number;
  discountedPrice?: number | null;
  imagePath: string;
  description: string;
  categoryId: number;
}