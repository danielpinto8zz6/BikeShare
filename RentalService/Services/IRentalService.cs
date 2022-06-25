using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models.Dtos;

namespace RentalService.Services
{
    public interface IRentalService
    {
        Task<IEnumerable<RentalDto>> GetByUsernameAsync(string username);
        Task<IEnumerable<RentalDto>> GetAllAsync();
        Task<RentalDto> GetByIdAsync(Guid id);
        Task<RentalDto> CreateAsync(RentalDto rentalDto);
        Task<RentalDto> UpdateAsync(Guid id, RentalDto rentalDto);
    }
}