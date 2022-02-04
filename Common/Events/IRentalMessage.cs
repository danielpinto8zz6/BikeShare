using System;
using Common.Models.Dtos;

namespace Common.Events
{
    public interface IRentalMessage
    {
        Guid CorrelationId { get;  }

        RentalDto Rental { get; }
    }
}