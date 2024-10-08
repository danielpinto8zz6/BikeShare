using System;
using Common.Models.Dtos;

namespace PaymentCalculatorService.Services
{
    public class PaymentCalculatorService : IPaymentCalculatorService
    {
        private const double PricePerMinute = 0.10;
        private const double RentalPrice = 0.5;

        public double Calculate(PaymentDto paymentDto)
        {
            var value = RentalPrice + (paymentDto.Duration * PricePerMinute);
            
            return Math.Round(value, 2);
        }
    }
}