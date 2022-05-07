using Common.Models.Dtos;

namespace Common.Models.Events.Payment;

public interface IPaymentMessage
{
    Guid CorrelationId { get;  }

    PaymentDto Payment { get; }
}