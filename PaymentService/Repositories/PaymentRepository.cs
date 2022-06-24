using System;
using System.Threading.Tasks;
using Common.Services.Repositories;
using MongoDB.Driver;
using PaymentService.Models.Entities;

namespace PaymentService.Repositories
{
    public class PaymentRepository : MongoDbRepository, IPaymentRepository
    {
        private readonly IMongoDatabase _mongoDatabase;

        public PaymentRepository(IMongoClient mongoClient, string databaseName) : base(mongoClient, databaseName)
        {
            _mongoDatabase = mongoClient.GetDatabase(databaseName);
        }

        public Task<Payment> GetByRentalIdAsync(Guid rentalId)
        {
            var mongoCollection = _mongoDatabase.GetCollection<Payment>(nameof(Payment));

            var filter = Builders<Payment>.Filter.Eq(payment => payment.RentalId, rentalId);

            return mongoCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}