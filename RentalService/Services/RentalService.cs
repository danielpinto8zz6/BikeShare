using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Models.Dtos;
using Common.Models.Events;
using Common.Models.Events.Rental;
using LSG.GenericCrud.Dto.Services;
using LSG.GenericCrud.Repositories;
using LSG.GenericCrud.Services;
using MassTransit;
using RentalService.Models.Entities;

namespace RentalService.Services;

public class RentalService : CrudServiceBase<Guid, RentalDto, Rental>, IRentalService
{
    private readonly IMapper _mapper;

    private readonly ICrudService<Guid, Rental> _service;

    private readonly IPublishEndpoint _publishEndpoint;

    public RentalService(
        ICrudService<Guid, Rental> service,
        ICrudRepository repository,
        IMapper mapper,
        IPublishEndpoint publishEndpoint
    ) : base(service, repository, mapper)
    {
        _service = service;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public override async Task<RentalDto> CreateAsync(RentalDto rentalDto)
    {
        var rental = _mapper.Map<Rental>(rentalDto);

        var createdEntity = await _service.CreateAsync(rental);

        var result = _mapper.Map<RentalDto>(createdEntity);

        await _publishEndpoint.Publish<IRentalSubmitted>(new
        {
            CorrelationId = Guid.NewGuid(),
            Rental = result
        });

        return rentalDto;
    }
}