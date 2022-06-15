namespace Common.Models.Dtos;

public interface IEntity<T>
{
    T Id { get; set; }
}