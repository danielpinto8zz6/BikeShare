using LSG.GenericCrud.Models;
using LSG.GenericCrud.Repositories;

namespace Common.Services.Repositories
{
    public interface IMongoDbRepository : ICrudRepository
    {
        Task<T2> UpdateAsync<T1, T2>(T1 id, T2 entity) where T2 : class, IEntity<T1>, new();
    }
}