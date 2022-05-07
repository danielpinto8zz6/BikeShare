using System;
using System.Threading.Tasks;
using BikeValidateService.Services;
using Common.Models.Dtos;
using LSG.GenericCrud.Services;

namespace BikeService.Services
{
    public class BikeValidateService : IBikeValidateService
    {
        private readonly ICrudService<Guid, BikeDto> _bikeService;

        public BikeValidateService(ICrudService<Guid, BikeDto> bikeService)
        {
            _bikeService = bikeService;
        }

        public async Task<bool> IsBikeValidAsync(RentalDto rentalDto)
        {
            if (rentalDto == null)
            {
                throw new Exception();
            }

            var bike = await _bikeService.GetByIdAsync(rentalDto.BikeId);
            if (bike == null)
            {
                return false;
            }

            //TODO: validation
            //return !string.IsNullOrWhiteSpace(rentalDto.BikeKey) && rentalDto.BikeKey == bike.Key;
            return true;
        }
    }
}