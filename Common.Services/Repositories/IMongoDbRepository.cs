using Common.Models.Dtos;

namespace Common.Services.Repositories
{
    public interface IMongoDbRepository
    {
        Task<T2> UpdateAsync<T1, T2>(T1 id, T2 entity) where T2 : class, IEntity<T1>, new();
        Task<IQueryable<T2>> GetAllAsync<T1, T2>() where T2 : class, IEntity<T1>, new();
        Task<T2> GetByIdAsync<T1, T2>(T1 id) where T2 : class, IEntity<T1>, new();
        Task<T2> CreateAsync<T1, T2>(T2 entity) where T2 : class, IEntity<T1>, new();
        Task<T2> DeleteAsync<T1, T2>(T1 id) where T2 : class, IEntity<T1>, new();
    }
}