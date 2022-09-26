using Common.Models.Dtos;
using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace TestingService.Controllers;

[ApiController]
[Route("[controller]")]
public class TestingController : ControllerBase
{
    private readonly IProducer<NotificationDto> _notificationProducer;
    
    public TestingController(
        IProducer<NotificationDto> notificationProducer)
    {
        _notificationProducer = notificationProducer;
    }

    [HttpPost("notification")]
    public IActionResult AddNotificationAsync([FromBody] NotificationDto notificationDto)
    {
        _notificationProducer.ProduceAsync(notificationDto);

        return Accepted();
    }
}