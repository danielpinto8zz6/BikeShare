using Common.Models.Dtos;
using Common.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace TestingService.Controllers;

[ApiController]
[Route("[controller]")]
public class TestingController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public TestingController(
        IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("notification")]
    public IActionResult AddNotificationAsync([FromBody] NotificationDto notificationDto)
    {
        _publishEndpoint.Publish(notificationDto);

        return Accepted();
    }
}