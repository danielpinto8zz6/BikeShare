using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BikeService.Models.Dtos;
using Common.Models.Dtos;
using LSG.GenericCrud.Services;

namespace BikeService.Services
{
    public interface IBikeService : ICrudService<Guid, BikeDto>
    {
        Task<IEnumerable<BikeDto>> GetNearByAsync(NearByBikesRequestDto nearByBikesRequestDto);
    }
}