using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models.Dtos;
using LSG.GenericCrud.Services;

namespace RentalService.Services
{
    public interface IRentalService : ICrudService<Guid, RentalDto>
    {
        Task<IEnumerable<RentalDto>> GetHistoryByUsernameAsync(string username);
    }
}