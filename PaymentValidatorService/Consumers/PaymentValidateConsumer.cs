using System;
using System.Threading.Tasks;
using Common.Models.Commands.Payment;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace PaymentValidatorService.Consumers
{
    public class PaymentValidateConsumer : IConsumer<IValidatePayment>
    {

        private readonly ILogger<PaymentValidateConsumer> _logger;

        public PaymentValidateConsumer(ILogger<PaymentValidateConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<IValidatePayment> context)
        {
            try
            {
                UpdatePaymentState(context.Message.Payment, PaymentStatus.Validated);

                return SendPaymentValidated(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating payment status");

                UpdatePaymentState(context.Message.Payment, PaymentStatus.ValidationFailed);

                return SendPaymentValidationFailed(context);
            }
        }

        private static async Task SendPaymentValidated(ConsumeContext<IValidatePayment> context)
        {
            var endpoint = await context.GetSendEndpoint(new Uri($"queue:{nameof(IPaymentValidated)}"));
            await endpoint.Send<IPaymentValidated>(new
            {
                context.CorrelationId,
                context.Message.Payment
            });
        }

        private static async Task SendPaymentValidationFailed(ConsumeContext<IValidatePayment> context)
        {
            var endpoint = await context.GetSendEndpoint(new Uri($"queue:{nameof(IPaymentValidationFailed)}"));
            await endpoint.Send<IPaymentValidationFailed>(new
            {
                context.CorrelationId,
                context.Message.Payment
            });
        }

        private static void UpdatePaymentState(PaymentDto payment, PaymentStatus status)
        {
            payment.Status = status;
        }
    }
}