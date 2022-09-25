using System;
using System.Threading.Tasks;
using FeedbackService.Models.Dtos;

namespace FeedbackService.Services;

public interface IFeedbackService
{
    Task<FeedbackDto> CreateAsync(FeedbackDto feedbackDto);
    Task<FeedbackDto> GetByRentalIdAsync(Guid rentalId);
}