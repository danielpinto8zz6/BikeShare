using System;
using System.Threading.Tasks;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events.Payment;
using MassTransit;
using PaymentService.Services;

namespace PaymentService.Consumers;

public class PaymentRequestConsumer : IConsumer<PaymentRequestDto>
{
    private readonly IPaymentService _paymentService;

    private readonly IPublishEndpoint _publishEndpoint;

    public PaymentRequestConsumer(
        IPaymentService paymentService,
        IPublishEndpoint publishEndpoint)
    {
        _paymentService = paymentService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<PaymentRequestDto> context)
    {
        var paymentRequest = context.Message;
        var payment = new PaymentDto
        {
            Status = PaymentStatus.Requested,
            RentalId = paymentRequest.RentalId,
            Duration = (paymentRequest.EndDate - paymentRequest.StartDate).TotalMinutes
        };

        var paymentDto = await _paymentService.CreateAsync(payment);

        await _publishEndpoint.Publish<IPaymentRequested>(new
        {
            CorrelationId = Guid.NewGuid(),
            Payment = paymentDto
        });
    }
}