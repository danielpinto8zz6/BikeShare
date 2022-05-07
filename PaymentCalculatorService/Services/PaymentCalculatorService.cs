using System;
using Common.Models.Dtos;

namespace PaymentCalculatorService.Services
{
    public class PaymentCalculatorService : IPaymentCalculatorService
    {
        public double Calculate(PaymentDto paymentDto)
        {
            return 2;
        }
    }
}