using System;

namespace BackendApi.DTOs;

public class DiscountDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int Percentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? ProductId { get; set; }

    public string? ProductName { get; set; }

    public int? CategoryId { get; set; }

    public string? CategoryName { get; set; }
}