using System.ComponentModel.DataAnnotations;

namespace BackendApi.Dtos.Admin;

public class AdminCategoryUpdateDto
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;
}