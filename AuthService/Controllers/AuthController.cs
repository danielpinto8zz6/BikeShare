using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Services.AuthService _authService;

        public AuthController(Services.AuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post([FromBody] AuthRequestDto user)
        {
            var token = _authService.Authenticate(user.Login, user.Password);

            if (token == null)
                return BadRequest(new {message = "Username of password incorrect"});

            return Ok(new {Token = token});
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_authService.AgentFromLogin(HttpContext.User.Identity?.Name));
        }
    }
}