using System.Threading.Tasks;
using AuthService.Helpers;
using AuthService.Models.Dtos;

namespace AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;

        private readonly IPasswordService _passwordService;

        private readonly IJwtService _jwtService;

        public AuthService(IUserService userService, IPasswordService passwordService, IJwtService jwtService)
        {
            _userService = userService;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> AuthenticateAsync(AuthRequestDto authRequestDto)
        {
            var user = await _userService.GetByUsernameAsync(authRequestDto.Username);
            if (user == null)
                return default;

            if (!_passwordService.Matches(authRequestDto.Password, user.PasswordHash))
                return default;


            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token
            };
        }
    }
}