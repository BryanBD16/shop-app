namespace BackendApi.Dtos.Admin;

public class AdminProductUpdateDto
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string ImagePath { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int StockQuantity { get; set; }

    public bool IsPublished { get; set; }
}
