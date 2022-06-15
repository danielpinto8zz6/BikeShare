using System;
using System.Threading.Tasks;
using AutoMapper;
using BikeService.Models.Entities;
using Common.Extensions.Exceptions;
using Common.Models.Dtos;
using Common.Services.Repositories;

namespace BikeService.Services;

public class BikeService : IBikeService
{
    private readonly IMongoDbRepository _repository;

    private readonly IMapper _mapper;

    public BikeService(
        IMongoDbRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BikeDto> GetByIdAsync(Guid id)
    {
        var bike = await _repository.GetByIdAsync<Guid, Bike>(id);
        if (bike == null)
            throw new NotFoundException();

        return _mapper.Map<BikeDto>(bike);
    }

    public async Task<BikeDto> CreateAsync(BikeDto bikeDto)
    {
        var entity = _mapper.Map<Bike>(bikeDto);

        var result = await _repository.CreateAsync<Guid, Bike>(entity);

        return _mapper.Map<BikeDto>(result);
    }

    public async Task<BikeDto> UpdateAsync(Guid id, BikeDto bikeDto)
    {
        var bike = await _repository.GetByIdAsync<Guid, Bike>(id);
        if (bike == null)
            throw new NotFoundException();

        bike = _mapper.Map<Bike>(bikeDto);

        var result = await _repository.UpdateAsync(id, bike);

        return _mapper.Map<BikeDto>(result);
    }
}