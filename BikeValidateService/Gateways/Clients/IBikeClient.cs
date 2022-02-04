using System;
using System.Threading.Tasks;
using Common.Models.Dtos;
using Refit;

namespace BikeValidateService.Gateways.Clients
{
    public interface IBikeClient
    {
        [Get("/bikes/{id}")]
        Task<BikeDto> GetByIdAsync(Guid id);
    }
}