using System;
using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs;

public class AdminDiscountCreateDto
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Range(0, 100)]
    public int Percentage { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? ProductId { get; set; }

    public int? CategoryId { get; set; }
}