using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Services.Repositories;
using FeedbackService.Models.Dtos;
using FeedbackService.Models.Entities;

namespace FeedbackService.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IMongoDbRepository _repository;
    
    private readonly IMapper _mapper;

    public FeedbackService(
        IMongoDbRepository repository, 
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
}