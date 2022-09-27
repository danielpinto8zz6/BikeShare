using System.Threading.Tasks;
using AuthService.Models.Dtos;
using Common.Services;

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
                return null;

            if (!_passwordService.Matches(authRequestDto.Password, user.PasswordHash))
                return null;


            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token
            };
        }
    }
}