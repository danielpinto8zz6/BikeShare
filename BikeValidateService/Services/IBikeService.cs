using System;
using System.Threading.Tasks;
using Common.Models.Dtos;

namespace BikeValidateService.Services
{
    public interface IBikeService
    {
        Task<BikeDto> GetByIdAsync(Guid id);
    }
}