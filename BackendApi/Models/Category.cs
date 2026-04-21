using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    // Navigation property (one-to-many)
public ICollection<Product> Products { get; set; } = new List<Product>();

public ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}