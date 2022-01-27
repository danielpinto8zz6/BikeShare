using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BikeService.Models.Dtos;
using BikeService.Models.Entities;
using BikeService.Repositories;
using Common.Enums;
using Common.Models.Dtos;
using LSG.GenericCrud.Dto.Services;
using LSG.GenericCrud.Services;

namespace BikeService.Services
{
    public class BikeService : CrudServiceBase<Guid, BikeDto, Bike>, IBikeService
    {
        private readonly IBikeRepository _repository;

        private readonly IMapper _mapper;

        public BikeService(ICrudService<Guid, Bike> service, IBikeRepository repository, IMapper mapper) : base(service,
            repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BikeDto>> GetNearByAsync(NearByBikesRequestDto nearByBikesRequestDto)
        {
            var result = await _repository.GetNearByAsync(nearByBikesRequestDto);

            if (nearByBikesRequestDto.OnlyAvailable)
                result = result.Where(item => item.Status == BikeStatus.Available);

            return _mapper.Map<IEnumerable<BikeDto>>(result);
        }

        public override async Task<BikeDto> UpdateAsync(Guid id, BikeDto dto)
        {
            var updatedEntity = await _repository.UpdateAsync(id, _mapper.Map<Bike>(dto));

            return _mapper.Map<BikeDto>(updatedEntity);
        }
    }
}