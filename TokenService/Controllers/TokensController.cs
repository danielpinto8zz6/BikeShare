using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TokenService.Models.Dto;
using TokenService.Services;

namespace TokenService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokensController : ControllerBase
    {
        private readonly ILogger<TokensController> _logger;

        private readonly ITokenService _tokenService;

        public TokensController(
            ILogger<TokensController> logger,
            ITokenService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetTokenByKeyAsync(string key)
        {
            var token = await _tokenService.GetTokenByKeyAsync(key);

            return Ok(token);
        }
        
        [HttpPut]                                                           
        public async Task<IActionResult> PutTokenAsync([FromBody] TokenRequestDto tokenRequestDto)                    
        {                                                                   
            await _tokenService.PutTokenAsync(tokenRequestDto);        
                                                                       
            return Ok();                                                             
        }                                                                   

    }
}