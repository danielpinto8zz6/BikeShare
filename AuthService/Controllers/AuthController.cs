using System.Threading.Tasks;
using AuthService.Models.Dtos;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthRequestDto authRequestDto)
        {
            var authResponseDto = await _authService.AuthenticateAsync(authRequestDto);
            if (authResponseDto == null)
                return BadRequest(new {message = "Username of password incorrect"});

            return Ok(authResponseDto);
        }
    }
}