using System;
using System.Threading.Tasks;
using Common.Models.Dtos;

namespace BikeValidateService.Services
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

            var bike = await _bikeService.GetByIdAsync(rentalDto.DockId);
            if (bike == null)
            {
                throw new Exception();
            }

            return !string.IsNullOrWhiteSpace(rentalDto.BikeKey) && rentalDto.BikeKey == bike.Key;
        }
    }
}