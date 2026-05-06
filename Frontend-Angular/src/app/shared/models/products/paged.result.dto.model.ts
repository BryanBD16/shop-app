export interface PagedResultDto<T> {
  items: T[];
  currentPage: number;
  totalPages: number;
  totalItems: number;
}