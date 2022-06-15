using System;
using System.Threading.Tasks;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events.Payment;
using LSG.GenericCrud.Services;
using MassTransit;

namespace PaymentService.Consumers;

public class PaymentRequestConsumer : IConsumer<PaymentRequestDto>
{
    private readonly ICrudService<Guid, PaymentDto> _paymentService;

    private readonly IPublishEndpoint _publishEndpoint;

    public PaymentRequestConsumer(
        ICrudService<Guid, PaymentDto> paymentService,
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
            StartDate = paymentRequest.StartDate,
            EndDate = paymentRequest.EndDate,
            RentalId = paymentRequest.RentalId
        };

        var paymentDto = await _paymentService.CreateAsync(payment);

        await _publishEndpoint.Publish<IPaymentRequested>(new
        {
            CorrelationId = Guid.NewGuid(),
            Payment = paymentDto
        });
    }
}