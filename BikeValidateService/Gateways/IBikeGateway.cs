using System;
using System.Threading.Tasks;
using Common.Models.Dtos;

namespace BikeValidateService.Gateways
{
    public interface IBikeGateway
    {
        Task<BikeDto> GetByIdAsync(Guid id);
    }
}