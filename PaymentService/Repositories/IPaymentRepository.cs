using System;
using System.Threading.Tasks;
using Common.Services.Repositories;
using PaymentService.Models.Entities;

namespace PaymentService.Repositories
{
    public interface IPaymentRepository : IMongoDbRepository
    {
        Task<Payment> GetByRentalIdAsync(Guid rentalId);
    }
}