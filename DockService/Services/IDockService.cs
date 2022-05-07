using DockService.Models.Dtos;
using LSG.GenericCrud.Services;

namespace DockService.Services
{
    public interface IDockService : ICrudService<Guid, DockDto>
    {
        Task<IEnumerable<DockDto>> GetNearByDocksAsync(NearByDocksRequestDto nearByDocksRequestDto);

        Task<DockDto> GetByBikeId(Guid bikeId);
    }
}