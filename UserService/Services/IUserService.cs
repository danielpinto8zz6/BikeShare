using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models.Dtos;

namespace UserService.Services;

public interface IUserService
{
    Task<UserDto> GetByUsernameAsync(string username);
    Task<UserDto> CreateAsync(UserDto userDto);
    Task<UserDto> UpdateAsync(string username, UserDto userDto);
    Task AddCreditCardAsync(string username, CreditCardDto creditCardDto);
    Task<IEnumerable<CreditCardDto>> GetCreditCardsByUsernameAsync(string username);
    Task DeleteCreditCardByNumberAsync(string username, string creditCardNumber);
}