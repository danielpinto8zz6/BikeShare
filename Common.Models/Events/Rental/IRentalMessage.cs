using Common.Models.Dtos;

namespace Common.Models.Events.Rental;

public interface IRentalMessage
{
    Guid CorrelationId { get; set; }

    RentalDto Rental { get; set; }
}

public class RentalMessage : IRentalMessage
{
    public Guid CorrelationId { get; set; }
    public RentalDto Rental { get; set; }
}