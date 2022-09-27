using Common.Models.Dtos;
using FluentAssertions;
using NUnit.Framework;
using PaymentCalculatorService.Services;

namespace PaymentCalculatorService.Tests;

public class PaymentCalculatorServiceTests
{
    private IPaymentCalculatorService _paymentCalculatorService;
    
    [SetUp]
    public void Setup()
    {
        _paymentCalculatorService = new Services.PaymentCalculatorService();
    }

    [TestCase(10, 1.5)]
    [TestCase(20, 2.5)]
    [TestCase(30, 3.5)]
    [TestCase(1, 0.6)]
    [TestCase(0, 0.5)]
    public void Calculate_WithValidRequest_ShouldReturnExpectedValue(double duration, double expectedValue)
    {
        var date = DateTime.UtcNow;
        var payment = new PaymentDto
        {
            StartDate = date,
            EndDate = date.AddMinutes(duration)
        };
        
        var result = _paymentCalculatorService.Calculate(payment);

        result.Should().Be(expectedValue);
    }
}