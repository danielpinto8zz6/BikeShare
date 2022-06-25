using AutoMapper;
using Common.Extensions.Exceptions;
using Common.Models.Dtos;
using Common.TravelEvent.Repositories;

namespace Common.TravelEvent.Services;

public class TravelEventService : ITravelEventService
{
    private readonly ITravelEventRepository _repository;

    private readonly IMapper _mapper;

    public TravelEventService(
        ITravelEventRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TravelEventDto> CreateAsync(TravelEventDto travelEventDto)
    {
        var entity = _mapper.Map<Entities.TravelEvent>(travelEventDto);

        var result = await _repository.CreateAsync<Guid, Entities.TravelEvent>(entity);

        return _mapper.Map<TravelEventDto>(result);
    }

    public async Task<IEnumerable<TravelEventDto>> GetByRentalIdAsync(Guid rentalId)
    {
        var travelEvents = await _repository.GetByRentalIdAsync(rentalId);
        if (travelEvents == null)
            throw new NotFoundException();

        return _mapper.Map<IEnumerable<TravelEventDto>>(travelEvents);
    }
}