export interface DiscountDto {
  id: number;
  title: string;
  percentage: number;
  startDate: string;
  endDate?: string | null;
  productId?: number | null;
  productName?: string | null;
  categoryId?: number | null;
  categoryName?: string | null;
}