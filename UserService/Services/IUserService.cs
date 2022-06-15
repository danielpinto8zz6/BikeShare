using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models.Dtos;

namespace UserService.Services;

public interface IUserService
{
    Task<ApplicationUserDto> GetByUsernameAsync(string username);
    Task<ApplicationUserDto> CreateAsync(ApplicationUserDto applicationUserDto);
    Task<ApplicationUserDto> UpdateAsync(string username, ApplicationUserDto applicationUserDto);
    Task AddCreditCardAsync(string username, CreditCardDto creditCardDto);
    Task<IEnumerable<CreditCardDto>> GetCreditCardsByUsernameAsync(string username);
}