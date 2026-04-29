namespace BackendApi.DTOs
{
    public class AdminProductListItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal OriginalPrice { get; set; }

        public decimal? DiscountedPrice { get; set; }

        public string ImagePath { get; set; } = string.Empty;

        public int StockQuantity { get; set; }

        public bool IsPublished { get; set; }

        public int CategoryId { get; set; }
    }
}
