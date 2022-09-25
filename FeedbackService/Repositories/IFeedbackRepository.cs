using System;
using System.Threading.Tasks;
using Common.Services.Repositories;
using FeedbackService.Models.Entities;

namespace FeedbackService.Repositories;

public interface IFeedbackRepository : IMongoDbRepository
{
    Task<Feedback> GetByRentalIdAsync(Guid rentalId);
}