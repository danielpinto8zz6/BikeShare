using System;
using System.Threading.Tasks;
using BikeValidateService.Services;
using Common.Models.Dtos;

namespace BikeService.Services
{
    public class BikeValidateService : IBikeValidateService
    {
        private readonly IBikeService _bikeService;

        public BikeValidateService(IBikeService bikeService)
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