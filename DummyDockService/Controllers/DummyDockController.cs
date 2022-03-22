using DummyDockService.Models.Dtos;
using LSG.GenericCrud.Exceptions;
using LSG.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;

namespace DummyDockService.Controllers;

[ApiController]
[Route("[controller]")]
public class DummyDockController : ControllerBase
{
    private readonly ILogger<DummyDockController> _logger;
    
    private readonly ICrudService<Guid, DummyDockDto> _service;

    public DummyDockController(
        ILogger<DummyDockController> logger, 
        ICrudService<Guid, DummyDockDto> service)
    {
        _logger = logger;
        _service = service;
    }
    
    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<DummyDockDto>>> GetAll()
    {
        var dummyDockDtos = await _service.GetAllAsync();
        
        return Ok(dummyDockDtos);
    }

    [HttpGet("{id}")]
    public virtual async Task<ActionResult<DummyDockDto>> GetById(Guid id)
    {
        try
        {
            var dummyDockDto = await _service.GetByIdAsync(id);
            
            return Ok(dummyDockDto);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogError(ex, "Entity not found");
            
            return NotFound();
        }
    }
}