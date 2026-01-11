using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [MaxLength(255)]
    public string ImagePath { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int StockQuantity { get; set; }

    public bool IsPublished { get; set; } = true;
}
