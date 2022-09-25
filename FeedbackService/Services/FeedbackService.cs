using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Services.Repositories;
using FeedbackService.Models.Dtos;
using FeedbackService.Models.Entities;
using FeedbackService.Repositories;

namespace FeedbackService.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _repository;
    
    private readonly IMapper _mapper;

    public FeedbackService(
        IFeedbackRepository repository, 
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<FeedbackDto> CreateAsync(FeedbackDto feedbackDto)
    {
        var entity = _mapper.Map<Feedback>(feedbackDto);

        var result = await _repository.CreateAsync<Guid, Feedback>(entity);

        return _mapper.Map<FeedbackDto>(result);
    }
    
    
    public async Task<FeedbackDto> GetByRentalIdAsync(Guid rentalId)
    {
        var result = await _repository.GetByRentalIdAsync(rentalId);

        return _mapper.Map<FeedbackDto>(result);
    }
}