using System;
using System.Threading;
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

        public async Task Consume(ConsumeContext<IValidatePayment> context)
        {
            try
            {
                _logger.LogInformation($"Validate payment to {context.CorrelationId} was received");

                UpdatePaymentState(context.Message.Payment, PaymentStatus.Validated);

                // Sleep 3 sec to emulate payment validation
                await Task.Delay(3000);
                
                await SendPaymentValidated(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating payment status");

                UpdatePaymentState(context.Message.Payment, PaymentStatus.Failed);

                await SendPaymentValidationFailed(context);
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
            var endpoint = await context.GetSendEndpoint(new Uri($"queue:{nameof(IPaymentFailed)}"));
            await endpoint.Send<IPaymentFailed>(new
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