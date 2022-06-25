using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Extensions.Exceptions;
using Common.Models.Dtos;
using Common.Services.Repositories;
using UserService.Models.Entities;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly IMongoDbRepository _mongoDbRepository;

    private readonly IMapper _mapper;

    public UserService(IMongoDbRepository mongoDbRepository, IMapper mapper)
    {
        _mongoDbRepository = mongoDbRepository;
        _mapper = mapper;
    }

    public async Task<UserDto> GetByUsernameAsync(string username)
    {
        var user = await _mongoDbRepository.GetByIdAsync<string, ApplicationUser>(username);
        if (user == null)
            throw new NotFoundException();
        
        return _mapper.Map<UserDto>(user);
    }
    
    public async Task<UserDto> CreateAsync(
        UserDto userDto)
    {
        var entity = _mapper.Map<ApplicationUser>(userDto);
        
        var result = await _mongoDbRepository.CreateAsync<string, ApplicationUser>(entity);

        return _mapper.Map<UserDto>(result);
    }
    
    public async Task<UserDto> UpdateAsync(string username, UserDto userDto)
    {
        var user = await _mongoDbRepository.GetByIdAsync<string, ApplicationUser>(username);
        if (user == null)
            throw new NotFoundException();
        
        user = _mapper.Map<ApplicationUser>(userDto);
            
        var result = await _mongoDbRepository.UpdateAsync(username, user);

        return _mapper.Map<UserDto>(result);
    }

    public async Task AddCreditCardAsync(string username, CreditCardDto creditCardDto)
    {
        var user = await _mongoDbRepository.GetByIdAsync<string, ApplicationUser>(username);
        if (user == null)
            throw new NotFoundException();

        var creditCard = _mapper.Map<CreditCard>(creditCardDto);
        
        user.CreditCards ??= new List<CreditCard>();
        user.CreditCards.RemoveAll(c => c.CardNumber == creditCardDto.CardNumber);
        user.CreditCards.Add(creditCard);                                    
                                                                                   
        await _mongoDbRepository.UpdateAsync(user.Username, user);                        
    }

    public async Task<IEnumerable<CreditCardDto>> GetCreditCardsByUsernameAsync(string username)
    {
        var user = await GetByUsernameAsync(username);
        return user.CreditCards;
    }

    public async Task DeleteCreditCardByNumberAsync(string username, string creditCardNumber)
    {
        var user = await _mongoDbRepository.GetByIdAsync<string, ApplicationUser>(username);
        if (user == null)
            throw new NotFoundException();

        user.CreditCards = user.CreditCards
            .Where(c => c.CardNumber != creditCardNumber)
            .ToList();

        await _mongoDbRepository.UpdateAsync(user.Username, user);
    }
}