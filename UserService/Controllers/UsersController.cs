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
    
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Get([FromHeader(Name = "UserId")] string userId)
    {
        try
        {
            var user = await _service.GetByUsernameAsync(userId);
            return Ok(user);
        }
        catch (NotFoundException ex)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(
        [FromBody] UserDto userDto)
    {
        var user = await _service.CreateAsync(userDto);

        return CreatedAtAction("Get", new
        {
            username = user.Username
        }, user);
    }

    [HttpPut("me")]
    public async Task<IActionResult> Update(
        [FromHeader(Name = "UserId")] string userId,
        [FromBody] UserDto userDto)
    {
        try
        {
            await _service.UpdateAsync(userId, userDto);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound();
        }
    }

    [HttpPost("me/credit-cards")]
    public async Task<IActionResult> AddCreditCardAsync(
        [FromHeader(Name = "UserId")] string userId,
        [FromBody] CreditCardDto creditCardDto)
    {
        await _service.AddCreditCardAsync(userId, creditCardDto);

        return Ok();
    }

    [HttpGet("me/credit-cards")]
    public async Task<ActionResult<IEnumerable<CreditCardDto>>> GetCreditCardsByUsernameAsync(
        [FromHeader(Name = "UserId")] string userId)
    {
        var creditCards = await _service.GetCreditCardsByUsernameAsync(userId);

        return Ok(creditCards);
    }
    
    [HttpDelete("me/credit-cards/{creditCardNumber}")]
    public async Task<ActionResult<IEnumerable<CreditCardDto>>> DeleteCreditCardByNumberAsync(
        [FromHeader(Name = "UserId")] string userId,
        string creditCardNumber)
    {
        await _service.DeleteCreditCardByNumberAsync(userId, creditCardNumber);

        return Ok();
    }
}