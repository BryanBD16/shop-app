namespace BackendApi.DTOs;

public class ProductListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal OriginalPrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string ImagePath { get; set; } = "";
    public int CategoryId { get; set; }

}
