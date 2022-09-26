using Common.Models.Events.Rental;
using DockService.Models.Dtos;

namespace DockService.Services
{
    public interface IDockService
    {
        Task<IEnumerable<DockDto>> GetNearByDocksAsync(NearByDocksRequestDto nearByDocksRequestDto);
        Task<DockDto> GetByBikeId(Guid bikeId);
        Task<IEnumerable<DockDto>> GetAllAsync();
        Task<DockDto> GetByIdAsync(Guid id);
        Task<DockDto> CreateAsync(DockDto entity);
        Task<DockDto> UpdateAsync(Guid id, DockDto entity);
        Task<DockDto> DeleteAsync(Guid id);
    }
}