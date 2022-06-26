using AutoMapper;
using DockService.Models.Dtos;
using DockService.Models.Entities;
using DockService.Repositories;
using Nominatim.API.Geocoders;
using Nominatim.API.Models;

namespace DockService.Services
{
    public class DockService : IDockService
    {
        private readonly IDockRepository _repository;

        private readonly IMapper _mapper;

        private readonly ReverseGeocoder _reverseGeocoder;

        public DockService(IDockRepository repository, IMapper mapper, ReverseGeocoder reverseGeocoder)
        {
            _repository = repository;
            _mapper = mapper;
            _reverseGeocoder = reverseGeocoder;
        }

        public async Task<IEnumerable<DockDto>> GetNearByDocksAsync(NearByDocksRequestDto nearByDocksRequestDto)
        {
            var results = await _repository.GetNearByDocksAsync(nearByDocksRequestDto);

            switch (nearByDocksRequestDto.FilterStatus)
            {
                case DockStatus.WithBike:
                    results = results.Where(item => item.BikeId != null);
                    break;
                case DockStatus.WithoutBike:
                    results = results.Where(item => item.BikeId == null);
                    break;
            };

            foreach (var dock in results)
            {
                if (dock.Coordinates != null)
                    dock.Address = await GetAddressFromCoordinatesAsync(
                        dock.Coordinates.Latitude, dock.Coordinates.Longitude);
            }

            return _mapper.Map<IEnumerable<DockDto>>(results);
        }

        public async Task<DockDto> GetByBikeId(Guid bikeId)
        {
            var result = await _repository.GetByBikeId(bikeId);

            if (result.Coordinates != null)
                result.Address = await GetAddressFromCoordinatesAsync(
                    result.Coordinates.Latitude, result.Coordinates.Longitude);

            return _mapper.Map<DockDto>(result);
        }

        public async Task<IEnumerable<DockDto>> GetAllAsync()
        {
            var results = await _repository.GetAllAsync<Guid, Dock>();

            foreach (var dock in results)
            {
                if (dock.Coordinates != null)
                    dock.Address = await GetAddressFromCoordinatesAsync(
                        dock.Coordinates.Latitude, dock.Coordinates.Longitude);
            }

            return _mapper.Map<IEnumerable<DockDto>>(results);
        }

        public async Task<DockDto> GetByIdAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync<Guid, Dock>(id);

            if (result.Coordinates != null)
                result.Address = await GetAddressFromCoordinatesAsync(
                    result.Coordinates.Latitude, result.Coordinates.Longitude);

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

        public async Task<DockDto> DeleteAsync(Guid id)
        {
            var result = await _repository.DeleteAsync<Guid, Dock>(id);

            return _mapper.Map<DockDto>(result);
        }

        private async Task<string?> GetAddressFromCoordinatesAsync(double latitude, double longitude)
        {
            var reverseGeocodeRequest = new ReverseGeocodeRequest
            {
                Latitude = latitude,
                Longitude = longitude
            };

            try
            {
                var reverseGeocode = await _reverseGeocoder.ReverseGeocode(reverseGeocodeRequest);

                return reverseGeocode?.DisplayName;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}