using System;
using System.Threading.Tasks;
using BikeValidateService.Gateways.Clients;
using Common.Models.Dtos;

namespace BikeValidateService.Gateways
{
    public class BikeGateway : IBikeGateway
    {
        private readonly IBikeClient _bikeClient;

        public BikeGateway(IBikeClient bikeClient)
        {
            _bikeClient = bikeClient;
        }
        
        public Task<BikeDto> GetByIdAsync(Guid id)
        {
            return _bikeClient.GetByIdAsync(id);
        }
    }
}