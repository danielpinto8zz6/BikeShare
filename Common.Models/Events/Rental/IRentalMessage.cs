using Common.Models.Dtos;

namespace Common.Models.Events.Rental;

public interface IRentalMessage
{
    Guid CorrelationId { get; set; }

    RentalDto Rental { get; set; }
}