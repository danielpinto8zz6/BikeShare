using System;
using System.Collections.Generic;
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
    public virtual async Task<ActionResult<IEnumerable<BikeDto>>> GetAllAsync()
    {
        var result = await _service.GetAllAsync();

        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<BikeDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<BikeDto>> CreateAsync(
        [FromBody] BikeDto bikeDto)
    {
        var result = await _service.CreateAsync(bikeDto);

        return CreatedAtAction("GetById", new
        {
            id = result.Id
        }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] BikeDto bikeDto)
    {
        try
        {
            await _service.UpdateAsync(id, bikeDto);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound();
        }
    }
}