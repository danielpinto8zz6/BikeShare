using Common.Models.Dtos;

namespace PaymentCalculatorService.Services
{
    public interface IPaymentCalculatorService
    {
        double Calculate(PaymentDto paymentDto);
    }
}