using Common.Models.Dtos;
using DockInternalServiceEmulator.Models.Dtos;
using DockInternalServiceEmulator.Services;
using Microsoft.AspNetCore.Mvc;

namespace DockInternalServiceEmulator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DockInternalController : ControllerBase
{
    private readonly IDockInternalService _dockInternalService;

    public DockInternalController(IDockInternalService dockInternalService)
    {
        _dockInternalService = dockInternalService;
    }

    [HttpPost("attach-bike")]
    public async Task<ActionResult<BikeDto>> AttachBikeAsync(
        [FromBody] BikeAttachRequestDto bikeAttachRequestDto)
    {
        await _dockInternalService.AttachBikeAsync(bikeAttachRequestDto);

        return Ok(); 
    }
}