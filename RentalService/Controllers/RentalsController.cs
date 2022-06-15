using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Extensions.Exceptions;
using Common.Models.Dtos;
using LSG.GenericCrud.Exceptions;
using Microsoft.AspNetCore.Mvc;
using RentalService.Services;

namespace RentalService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RentalDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var rental = await _rentalService.GetByIdAsync(id);
                return Ok(rental);
            }
            catch (NotFoundException ex)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<ActionResult<RentalDto>> GetAllAsync()
        {
            var rental = await _rentalService.GetAllAsync();
            return Ok(rental);
        }

        [HttpPost]
        public async Task<ActionResult<RentalDto>> Create([FromBody] RentalDto rentalDto)
        {
            rentalDto.Username = Request.Headers["UserId"];

            var result = await _rentalService.CreateAsync(rentalDto);

            return CreatedAtAction("GetById", new
            {
                id = result.Id
            }, result);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<RentalDto>> UpdateAsync(
            [FromRoute]Guid id, 
            [FromBody] RentalDto rentalDto)
        {
            rentalDto.Username = Request.Headers["UserId"];
            
            try
            {
                await _rentalService.UpdateAsync(id, rentalDto);

                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound();
            }
        }

        [HttpGet("history/{username}")]
        public async Task<ActionResult<IEnumerable<RentalDto>>> GetHistoryByUsername([FromRoute] string username)
        {
            var rentals = await _rentalService.GetHistoryByUsernameAsync(username);

            return Ok(rentals);
        }
    }
}