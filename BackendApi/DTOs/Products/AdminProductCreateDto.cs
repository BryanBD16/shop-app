using System.ComponentModel.DataAnnotations;

namespace BackendApi.Dtos.Admin;

public class AdminProductCreateDto
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [StringLength(255)]
    public string ImagePath { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [Required]
    public bool IsPublished { get; set; } 

    [Required]
    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}
