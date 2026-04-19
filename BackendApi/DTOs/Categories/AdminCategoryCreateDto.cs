using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs;

public class AdminCategoryCreateDto
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; }
}