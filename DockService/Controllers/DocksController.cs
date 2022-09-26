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

        private readonly IDockManagerService _dockManagerService;

        public DocksController(IDockService service, IDockManagerService dockManagerService)
        {
            _service = service;
            _dockManagerService = dockManagerService;
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
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
        
        [HttpGet("bike/{bikeId}")]
        public async Task<ActionResult<DockDto>> GetByBikeIdAsync(Guid bikeId)
        {
            try
            {
                var result = await _service.GetByBikeId(bikeId);
                return Ok(result);
            }
            catch (NotFoundException)
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
            catch (NotFoundException)
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
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
        
        [HttpPost("lock-bike")]
        public async Task<ActionResult<BikeDto>> LockBikeAsync(
            [FromBody] BikeLockRequestDto bikeLockRequestDto)
        {
            try
            {
                await _dockManagerService.LockBikeAsync(bikeLockRequestDto);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}