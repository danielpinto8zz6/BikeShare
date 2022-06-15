using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models.Dtos;
using LSG.GenericCrud.Controllers;
using Microsoft.AspNetCore.Mvc;
using RentalService.Services;

namespace RentalService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalsController : CrudControllerBase<Guid, RentalDto>
    {
        private readonly IRentalService _rentalService;

        public RentalsController(IRentalService rentalService) : base(rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpPost]
        public override async Task<ActionResult<RentalDto>> Create([FromBody] RentalDto rentalDto)
        {
            rentalDto.Username = Request.Headers["UserId"];

            var result = await _rentalService.CreateAsync(rentalDto);

            return CreatedAtAction("GetById", new
            {
                id = result.Id
            }, result);
        }
        
        [HttpGet("history/{username}")]
        public async Task<ActionResult<IEnumerable<RentalDto>>> GetHistoryByUsername([FromRoute] string username)
        {
            var rentals = await _rentalService.GetHistoryByUsernameAsync(username);
            
            return Ok(rentals);
        }
    }
}