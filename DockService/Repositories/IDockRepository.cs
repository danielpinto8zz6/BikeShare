using Common.Services.Repositories;
using DockService.Models.Dtos;
using DockService.Models.Entities;
using MongoDB.Driver;

namespace DockService.Repositories
{
    public interface IDockRepository : IMongoDbRepository
    {
        Task<IQueryable<Dock>> GetNearByDocksAsync(NearByDocksRequestDto nearByDocksRequestDto);
        
        Task<Dock> GetByBikeId(Guid bikeId);
    }
}