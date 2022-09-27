using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models.Dtos;

namespace BikeService.Services;

public interface IBikeService
{
    Task<BikeDto> GetByIdAsync(Guid id);
    Task<BikeDto> CreateAsync(BikeDto bikeDto);
    Task<BikeDto> UpdateAsync(Guid id, BikeDto bikeDto);
    Task<IEnumerable<BikeDto>> GetAllAsync();
    Task<bool> ExistAsync(Guid id);
}