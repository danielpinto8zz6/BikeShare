using AutoMapper;
using DockService.Models.Dtos;
using DockService.Models.Entities;
using DockService.Repositories;

namespace DockService.Services
{
    public class DockService : IDockService
    {
        private readonly IDockRepository _repository;

        private readonly IMapper _mapper;

        public DockService(IDockRepository repository, IMapper mapper)
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

        public async Task<IEnumerable<DockDto>> GetAllAsync()
        {
            var result = await _repository.GetAllAsync<Guid, Dock>();

            return _mapper.Map<IEnumerable<DockDto>>(result);
        }

        public async Task<DockDto> GetByIdAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync<Guid, Dock>(id);

            return _mapper.Map<DockDto>(result);
        }

        public async Task<DockDto> CreateAsync(DockDto dockDto)
        {
            var result = await _repository.CreateAsync<Guid, Dock>(_mapper.Map<Dock>(dockDto));

            return _mapper.Map<DockDto>(result);
        }

        public async Task<DockDto> UpdateAsync(Guid id, DockDto dockDto)
        {
            var result = await _repository.UpdateAsync(id, _mapper.Map<Dock>(dockDto));

            return _mapper.Map<DockDto>(result);
        }

        public Task<DockDto> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<DockDto> CopyAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool AutoCommit { get; set; }
    }
}