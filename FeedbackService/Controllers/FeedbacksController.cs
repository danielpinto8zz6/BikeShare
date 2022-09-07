using System.Threading.Tasks;
using FeedbackService.Models.Dtos;
using FeedbackService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FeedbackService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        public FeedbacksController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        
        [HttpPost]
        public async Task<ActionResult<FeedbackDto>> CreateAsync(
            [FromHeader(Name = "UserId")] string userId,
            [FromBody] FeedbackDto feedbackDto)
        {
            feedbackDto.Username = userId;
            
            var result = await _feedbackService.CreateAsync(feedbackDto);

            return Created("", result);
        }
    }
}