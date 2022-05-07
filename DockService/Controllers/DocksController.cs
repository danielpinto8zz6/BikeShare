using Common.Models.Dtos;
using DockService.Models.Dtos;
using DockService.Services;
using LSG.GenericCrud.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DockService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocksController : CrudControllerBase<Guid, DockDto>
    {
        private readonly IDockService _service;

        public DocksController(IDockService service) : base(service)
        {
            _service = service;
        }

        [HttpGet("nearby")]
        public virtual async Task<ActionResult<IEnumerable<DockDto>>> GetNearBy(
            [FromQuery] NearByDocksRequestDto nearByBikesRequestDto)
        {
            var result = await _service.GetNearByDocksAsync(nearByBikesRequestDto);

            return Ok(result);
        }
    }
}