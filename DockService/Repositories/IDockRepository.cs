using Common.Repositories;
using DockService.Models.Dtos;
using DockService.Models.Entities;

namespace DockService.Repositories
{
    public interface IDockRepository : IMongoDbRepository
    {
        Task<IQueryable<Dock>> GetNearByDocksAsync(NearByDocksRequestDto nearByDocksRequestDto);
    }
}