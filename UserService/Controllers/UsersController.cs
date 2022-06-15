using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Extensions.Exceptions;
using Common.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<ApplicationUserDto>> GetByUsername(string username)
    {
        try
        {
            var user = await _service.GetByUsernameAsync(username);
            return Ok(user);
        }
        catch (NotFoundException ex)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationUserDto>> Create(
        [FromBody] ApplicationUserDto applicationUserDto)
    {
        var user = await _service.CreateAsync(applicationUserDto);

        return CreatedAtAction("GetByUsername", new
        {
            username = user.Username
        }, user);
    }

    [HttpPut("{username}")]
    public async Task<IActionResult> Update(
        string username,
        [FromBody] ApplicationUserDto applicationUserDto)
    {
        try
        {
            await _service.UpdateAsync(username, applicationUserDto);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound();
        }
    }

    [HttpPost("{username}/credit-card")]
    public async Task<IActionResult> AddCreditCardAsync(
        [FromRoute] string username,
        [FromBody] CreditCardDto creditCardDto)
    {
        await _service.AddCreditCardAsync(username, creditCardDto);

        return Ok();
    }

    [HttpGet("{username}/credit-card")]
    public async Task<ActionResult<IEnumerable<CreditCardDto>>> GetCreditCardsByUsernameAsync(string username)
    {
        var creditCards = await _service.GetCreditCardsByUsernameAsync(username);

        return Ok(creditCards);
    }
}