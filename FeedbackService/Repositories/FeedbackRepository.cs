using System;
using System.Threading.Tasks;
using Common.Services.Repositories;
using FeedbackService.Models.Entities;
using MongoDB.Driver;

namespace FeedbackService.Repositories;

public class FeedbackRepository : MongoDbRepository, IFeedbackRepository
{
    private readonly IMongoDatabase _mongoDatabase;

    public FeedbackRepository(IMongoClient mongoClient, string databaseName) : base(mongoClient, databaseName)
    {
        _mongoDatabase = mongoClient.GetDatabase(databaseName);
    }

    public Task<Feedback> GetByRentalIdAsync(Guid rentalId)
    {
        var mongoCollection = _mongoDatabase.GetCollection<Feedback>(nameof(Feedback));

        var filter = Builders<Feedback>.Filter.Eq(feedback => feedback.RentalId, rentalId);

        return mongoCollection.Find(filter).FirstOrDefaultAsync();
    }
}