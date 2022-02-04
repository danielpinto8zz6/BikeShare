using AutoMapper;
using Common.Models.Dtos;
using DockService.Models.Dtos;
using DockService.Models.Entities;
using DockService.Repositories;
using LSG.GenericCrud.Dto.Services;
using LSG.GenericCrud.Services;

namespace DockService.Services
{
    public class DockService : CrudServiceBase<Guid, DockDto, Dock>, IDockService
    {
        private readonly IDockRepository _repository;

        private readonly IMapper _mapper;

        public DockService(ICrudService<Guid, Dock> service, IDockRepository repository, IMapper mapper) : base(service,
            repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DockDto>> GetNearByDocksAsync(NearByDocksRequestDto nearByDocksRequestDto)
        {
            var result = await _repository.GetNearByDocksAsync(nearByDocksRequestDto);

            if (nearByDocksRequestDto.OnlyAvailable)
                result = result.Where(item => item.BikeId != null);

            return _mapper.Map<IEnumerable<DockDto>>(result);
        }

        public override async Task<DockDto> UpdateAsync(Guid id, DockDto dto)
        {
            var updatedEntity = await _repository.UpdateAsync(id, _mapper.Map<Dock>(dto));

            return _mapper.Map<DockDto>(updatedEntity);
        }
    }
}