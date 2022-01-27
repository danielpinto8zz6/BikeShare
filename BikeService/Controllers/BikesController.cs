using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BikeService.Models.Dtos;
using BikeService.Services;
using Common.Models.Dtos;
using LSG.GenericCrud.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BikeService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BikesController : CrudControllerBase<Guid, BikeDto>
    {
        private readonly IBikeService _service;

        public BikesController(IBikeService service) : base(service)
        {
            _service = service;
        }

        [HttpGet("nearby")]
        public virtual async Task<ActionResult<IEnumerable<BikeDto>>> GetNearBy(
            [FromQuery] NearByBikesRequestDto nearByBikesRequestDto)
        {
            var result = await _service.GetNearByAsync(nearByBikesRequestDto);

            return Ok(result);
        }
    }
}