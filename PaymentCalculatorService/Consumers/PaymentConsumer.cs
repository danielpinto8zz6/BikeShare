using System;
using System.Threading.Tasks;
using Common;
using Common.Models.Dtos;
using Common.Services;
using MassTransit;
using Newtonsoft.Json;
using PaymentCalculatorService.Services;

namespace PaymentCalculatorService.Consumers
{
    public class PaymentConsumer : IConsumer<PaymentDto>
    {
        private readonly IProducer<InvoiceDto> _producer;

        private readonly IPaymentCalculatorService _paymentCalculatorService;

        public PaymentConsumer(IProducer<InvoiceDto> producer, IPaymentCalculatorService paymentCalculatorService)
        {
            _producer = producer;
            _paymentCalculatorService = paymentCalculatorService;
        }

        public async Task Consume(ConsumeContext<PaymentDto> context)
        {
            Console.WriteLine("Message received:");
            Console.WriteLine(JsonConvert.SerializeObject(context.Message));

            var payment = _paymentCalculatorService.Calculate(context.Message);

        }
    }
}