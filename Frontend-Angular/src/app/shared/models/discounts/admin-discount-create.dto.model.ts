export interface AdminDiscountCreateDto {
  title: string;
  percentage: number;
  startDate: string;
  endDate?: string | null;
  productId?: number | null;
  categoryId?: number | null;
}