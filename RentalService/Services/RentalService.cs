using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Extensions.Exceptions;
using Common.Models.Dtos;
using Common.Models.Events.Rental;
using Common.Services;
using Common.Services.Repositories;
using MassTransit;
using RentalService.Models.Entities;

namespace RentalService.Services;

public class RentalService : IRentalService
{
    private readonly IMapper _mapper;

    private readonly IMongoDbRepository _repository;

    private readonly IBus _bus;

    public RentalService(
        IMongoDbRepository repository,
        IMapper mapper,
        IBus bus)
    {
        _repository = repository;
        _mapper = mapper;
        _bus = bus;
    }

    public async Task<RentalDto> GetByIdAsync(Guid id)
    {
        var rental = await _repository.GetByIdAsync<Guid, Rental>(id);
        if (rental == null)
            throw new NotFoundException();

        var rentalDto = _mapper.Map<RentalDto>(rental);

        return rentalDto;
    }

    public async Task<RentalDto> CreateAsync(RentalDto rentalDto)
    {
        var rental = _mapper.Map<Rental>(rentalDto);

        var createdEntity = await _repository.CreateAsync<Guid, Rental>(rental);

        var result = _mapper.Map<RentalDto>(createdEntity);

        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nameof(IRentalSubmitted)}"));

        await endpoint.Send<IRentalSubmitted>(new
        {
            CorrelationId = result.Id,
            Rental = result
        });

        return result;
    }

    public async Task<RentalDto> UpdateAsync(Guid id, RentalDto rentalDto)
    {
        var rental = await _repository.GetByIdAsync<Guid, Rental>(id);
        if (rental == null)
            throw new NotFoundException();

        rental = _mapper.Map<Rental>(rentalDto);
        var result = await _repository.UpdateAsync(id, rental);

        return _mapper.Map<RentalDto>(result);
    }

    public async Task<IEnumerable<RentalDto>> GetByUsernameAsync(string username)
    {
        var entities = await _repository.GetAllAsync<Guid, Rental>();
        var rentals = _mapper.Map<IEnumerable<RentalDto>>(entities.Where(i => i.Username == username));
        return rentals;
    }

    public async Task<IEnumerable<RentalDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync<Guid, Rental>();
        var rentals = _mapper.Map<IEnumerable<RentalDto>>(entities);

        return rentals;
    }
}