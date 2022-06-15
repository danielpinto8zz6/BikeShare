using System.Collections.Generic;
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

    public async Task<ApplicationUserDto> GetByUsernameAsync(string username)
    {
        var user = await _mongoDbRepository.GetByIdAsync<string, ApplicationUser>(username);
        if (user == null)
            throw new NotFoundException();
        
        return _mapper.Map<ApplicationUserDto>(user);
    }
    
    public async Task<ApplicationUserDto> CreateAsync(
        ApplicationUserDto applicationUserDto)
    {
        var entity = _mapper.Map<ApplicationUser>(applicationUserDto);
        
        var result = await _mongoDbRepository.CreateAsync<string, ApplicationUser>(entity);

        return _mapper.Map<ApplicationUserDto>(result);
    }
    
    public async Task<ApplicationUserDto> UpdateAsync(string username, ApplicationUserDto applicationUserDto)
    {
        var user = await _mongoDbRepository.GetByIdAsync<string, ApplicationUser>(username);
        if (user == null)
            throw new NotFoundException();
        
        user = _mapper.Map<ApplicationUser>(applicationUserDto);
            
        var result = await _mongoDbRepository.UpdateAsync(username, user);

        return _mapper.Map<ApplicationUserDto>(result);
    }

    public async Task AddCreditCardAsync(string username, CreditCardDto creditCardDto)
    {
        var user = await GetByUsernameAsync(username);                 
        user.CreditCards ??= new List<CreditCardDto>();                         
        user.CreditCards.Add(creditCardDto);                                    
                                                                                   
        await UpdateAsync(user.Username, user);                        
    }

    public async Task<IEnumerable<CreditCardDto>> GetCreditCardsByUsernameAsync(string username)
    {
        var user = await GetByUsernameAsync(username);
        return user.CreditCards;
    }
}