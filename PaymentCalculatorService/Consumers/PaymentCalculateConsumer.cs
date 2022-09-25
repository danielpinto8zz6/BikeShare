using System;
using System.Threading.Tasks;
using Common.Models.Commands.Payment;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;
using PaymentCalculatorService.Services;

namespace PaymentCalculatorService.Consumers
{
    public class PaymentCalculateConsumer : IConsumer<ICalculatePayment>
    {
        private readonly IPaymentCalculatorService _paymentCalculatorService;

        private readonly ILogger<PaymentCalculateConsumer> _logger;

        public PaymentCalculateConsumer(
            IPaymentCalculatorService paymentCalculatorService,
            ILogger<PaymentCalculateConsumer> logger)
        {
            _paymentCalculatorService = paymentCalculatorService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ICalculatePayment> context)
        {
            try
            {
                var paymentValue = _paymentCalculatorService.Calculate(context.Message.Payment);

                context.Message.Payment.Value = paymentValue;

                await SendPaymentCalculated(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating payment status");

                UpdatePaymentState(context.Message.Payment, PaymentStatus.CalculationFailed);

                await SendPaymentCalculationFailed(context);
            }
        }

        private static async Task SendPaymentCalculationFailed(ConsumeContext<ICalculatePayment> context)
        {
            await context.RespondAsync<IPaymentCalculationFailed>(new
            {
                context.CorrelationId,
                context.Message.Payment
            });
        }

        private static async Task SendPaymentCalculated(ConsumeContext<ICalculatePayment> context)
        {
            await context.RespondAsync<IPaymentCalculated>(new
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