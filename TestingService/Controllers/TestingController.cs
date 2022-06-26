using Common.Models.Dtos;
using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace TestingService.Controllers;

[ApiController]
[Route("[controller]")]
public class TestingController : ControllerBase
{
    private readonly IProducer<NotificationDto> _notificationProducer;

    private readonly IProducer<PaymentRequestDto> _paymentProducer;
    
    public TestingController(
        IProducer<NotificationDto> notificationProducer, IProducer<PaymentRequestDto> paymentProducer)
    {
        _notificationProducer = notificationProducer;
        _paymentProducer = paymentProducer;
    }

    [HttpPost("notification")]
    public IActionResult AddNotificationAsync([FromBody] NotificationDto notificationDto)
    {
        _notificationProducer.ProduceAsync(notificationDto);

        return Accepted();
    }
    
    [HttpPost("payment")]
    public IActionResult AddNotificationAsync([FromBody] PaymentRequestDto paymentRequestDto)
    {
        _paymentProducer.ProduceAsync(paymentRequestDto);

        return Accepted();
    }
}