using System;
using System.Threading.Tasks;
using AuthService.Models.Dtos;
using Common.Models.Dtos;

namespace AuthService.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IUserService _userService;

        private readonly IPasswordService _passwordService;

        public RegisterService(IUserService userService, IPasswordService passwordService)
        {
            _userService = userService;
            _passwordService = passwordService;
        }
        
        public async Task<ApplicationUserDto> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            //TODO: fluent validations
            
            var passwordHash = _passwordService.Hash(registerRequestDto.Password);

            var applicationUserDto = new ApplicationUserDto
            {
                Username = registerRequestDto.Username,
                PasswordHash = passwordHash
            };

            var result = await _userService.CreateAsync(applicationUserDto);
            if (result == null)
            {
                throw new Exception();
            }

            return result;
        }
    }
}