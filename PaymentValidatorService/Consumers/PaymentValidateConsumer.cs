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

        public async Task Consume(ConsumeContext<IValidatePayment> context)
        {
            try
            {
                UpdatePaymentState(context.Message.Payment, PaymentStatus.Validated);

                await SendPaymentValidated(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating payment status");

                UpdatePaymentState(context.Message.Payment, PaymentStatus.ValidationFailed);

                await SendPaymentValidationFailed(context);
            }
        }

        private static async Task SendPaymentValidated(ConsumeContext<IValidatePayment> context)
        {
            await context.RespondAsync<IPaymentValidated>(new
            {
                context.CorrelationId,
                context.Message.Payment
            });
        }

        private static async Task SendPaymentValidationFailed(ConsumeContext<IValidatePayment> context)
        {
            await context.RespondAsync<IPaymentValidationFailed>(new
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