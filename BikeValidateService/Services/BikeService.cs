using System;
using System.Threading.Tasks;
using BikeValidateService.Gateways;
using Common.Models.Dtos;

namespace BikeValidateService.Services
{
    public class BikeService : IBikeService
    {
        private readonly IBikeGateway _bikeGateway;

        public BikeService(IBikeGateway bikeGateway)
        {
            _bikeGateway = bikeGateway;
        }

        public Task<BikeDto> GetByIdAsync(Guid id)
        {
            return _bikeGateway.GetByIdAsync(id);
        }
    }
}