using Common.Extensions.Exceptions;
using Common.Models.Dtos;
using DockService.Models.Dtos;
using DockService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DockService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocksController : ControllerBase
    {
        private readonly IDockService _service;

        public DocksController(IDockService service)
        {
            _service = service;
        }

        [HttpGet("nearby")]
        public virtual async Task<ActionResult<IEnumerable<DockDto>>> GetNearByAsync(
            [FromQuery] NearByDocksRequestDto nearByBikesRequestDto)
        {
            var result = await _service.GetNearByDocksAsync(nearByBikesRequestDto);

            return Ok(result);
        }
        
        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<DockDto>>> GetAllAsync()
        {
            var result = await _service.GetAllAsync();

            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<DockDto>> GetByIdAsync(Guid id)
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
        
        [HttpGet("bike/{id}")]
        public async Task<ActionResult<DockDto>> GetByBikeIdAsync(Guid bikeId)
        {
            try
            {
                var result = await _service.GetByBikeId(bikeId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<DockDto>> CreateAsync(
            [FromBody] DockDto dockDto)
        {
            var result = await _service.CreateAsync(dockDto);

            return CreatedAtAction("GetById", new
            {
                id = result.Id
            }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            Guid id,
            [FromBody] DockDto dockDto)
        {
            try
            {
                await _service.UpdateAsync(id, dockDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound();
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound();
            }
        }
    }
}