namespace BackendApi.DTOs
{
    public class AdminProductListItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string ImagePath { get; set; } = string.Empty;

        public int StockQuantity { get; set; }

        public bool IsPublished { get; set; }
    }
}
