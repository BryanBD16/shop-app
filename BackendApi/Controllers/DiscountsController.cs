using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BackendApi.Dtos.Admin;
using BackendApi.DTOs;
using BackendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/discounts")]
public class DiscountsController : ControllerBase
{

    private readonly IDiscountService _discountService;

    public DiscountsController(IDiscountService discountService)
    {
        _discountService = discountService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var discount = await _discountService.GetDiscountByIdAsync(id);
        return Ok(discount);
    }
    
    [HttpPost("/api/admin/discounts")]
    public async Task<ActionResult> CreateDiscount([FromBody] AdminDiscountCreateDto dto)
    {
        var discountId = await _discountService.CreateDiscountAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = discountId }, discountId);
    }
    

}