namespace Common.Models.Dtos
{
    public interface IBaseEntity<T> : IEntity<T>, IDateEntity
    {
    }
}