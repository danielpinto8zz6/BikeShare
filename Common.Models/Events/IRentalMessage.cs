using Common.Models.Dtos;

namespace Common.Models.Events
{
    public interface IRentalMessage
    {
        Guid CorrelationId { get;  }

        RentalDto Rental { get; }
    }
}