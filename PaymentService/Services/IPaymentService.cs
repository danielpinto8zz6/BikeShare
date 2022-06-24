using System;
using System.Threading.Tasks;
using Common.Models.Dtos;

namespace PaymentService.Services;

public interface IPaymentService
{
    Task<PaymentDto> GetByIdAsync(Guid id);
    Task<PaymentDto> GetByRentalIdAsync(Guid rentalId);
    Task<PaymentDto> CreateAsync(PaymentDto paymentDto);
    Task<PaymentDto> UpdateAsync(Guid id, PaymentDto paymentDto); 
}