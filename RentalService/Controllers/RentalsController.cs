using System;
using System.Threading.Tasks;
using Common.Extensions.Exceptions;
using Common.Models.Dtos;
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
        public async Task<ActionResult<RentalDto>> GetAllAsync([FromHeader(Name = "UserId")] string userId)
        {
            var rentals = await _rentalService.GetByUsernameAsync(userId);
            
            return Ok(rentals);
        }

        [HttpPost]
        public async Task<ActionResult<RentalDto>> Create(
            [FromHeader(Name = "UserId")] string userId,
            [FromBody] RentalDto rentalDto)
        {
            rentalDto.Username = userId;

            var result = await _rentalService.CreateAsync(rentalDto);

            return CreatedAtAction("GetById", new
            {
                id = result.Id
            }, result);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<RentalDto>> UpdateAsync(
            [FromHeader(Name = "UserId")] string userId,
            [FromRoute]Guid id, 
            [FromBody] RentalDto rentalDto)
        {
            rentalDto.Username = userId;
            
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
    }
}