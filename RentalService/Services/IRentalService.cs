using System;
using Common.Models.Dtos;
using LSG.GenericCrud.Services;

namespace RentalService.Services
{
    public interface IRentalService : ICrudService<Guid, RentalDto>
    {
    }
}