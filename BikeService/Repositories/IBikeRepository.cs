using System.Linq;
using System.Threading.Tasks;
using BikeService.Models.Dtos;
using BikeService.Models.Entities;
using Common.Repositories;

namespace BikeService.Repositories
{
    public interface IBikeRepository : IMongoDbRepository
    {
        Task<IQueryable<Bike>> GetNearByAsync(NearByBikesRequestDto nearByBikesRequestDto);
    }
}