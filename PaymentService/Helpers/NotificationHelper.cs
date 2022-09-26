using System.Collections.Generic;
using Common.Models.Dtos;
using Common.Models.Events.Payment;
using MassTransit;
using PaymentService.Saga;

namespace PaymentService.Helpers;

public static class NotificationHelper
{
    private const string Event = "event";

    private const string PaymentId = "paymentId";

    public static PaymentNotificationDto GetPaymentSucceedNotification(
        BehaviorContext<PaymentState, IPaymentMessage> context)
    {
        return new PaymentNotificationDto
        {
            Username = context.Message.Payment.Username,
            Title = "Payment processed",
            Body = $"Payment of {context.Message.Payment.Value} processed successfully",
            Data = new Dictionary<string, string>
            {
                {Event, "payment-completed"},
                {PaymentId, context.Message.Payment.Id.ToString()}
            },
            PaymentId = context.Message.Payment.Id
        };
    }
    
    public static PaymentNotificationDto GetPaymentFailedNotification(
        BehaviorContext<PaymentState, IPaymentMessage> context)
    {
        return new PaymentNotificationDto
        {
            Username = context.Message.Payment.Username,
            Body = $"Payment of {context.Message.Payment.Value} failed, please check your card details",
            Title = "Payment failed",
            Data = new Dictionary<string, string>
            {
                {Event, "payment-failed"},
                {PaymentId, context.Message.Payment.Id.ToString()}
            },
            PaymentId = context.Message.Payment.Id
        };
    }
}