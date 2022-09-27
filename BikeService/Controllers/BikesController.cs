using System;
using System.Threading.Tasks;
using BikeService.Services;
using Common.Extensions.Exceptions;
using Common.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BikeService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BikesController : ControllerBase
{
    private readonly IBikeService _service;

    public BikesController(IBikeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _service.GetAllAsync();

        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] BikeDto bikeDto)
    {
        var result = await _service.CreateAsync(bikeDto);

        return CreatedAtAction("GetById", new
        {
            id = result.Id
        }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        [FromBody] BikeDto bikeDto)
    {
        try
        {
            await _service.UpdateAsync(id, bikeDto);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}