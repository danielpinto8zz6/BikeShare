using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models.Dtos;
using Common.Models.Events.Payment;
using MassTransit;
using PaymentService.Saga;

namespace PaymentService.Helpers;

public static class NotificationHelper
{
    private const string Event = "event";

    private const string PaymentId = "paymentId";

    public static async Task SendPaymentSucceedNotificationAsync(
        BehaviorContext<PaymentState, IPaymentValidationFailed> context)
    {
        var notificationDto = new PaymentNotificationDto
        {
            Username = context.Message.Payment.Username,
            Body = "Payment completed",
            Title = $"Payment of {context.Message.Payment.Value} processed successfully",
            Data = new Dictionary<string, string>
            {
                {Event, "payment-completed"},
                {PaymentId, context.Message.Payment.Id.ToString()}
            },
            PaymentId = context.Message.Payment.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }
    
    public static async Task SendPaymentFailedNotificationAsync(
        BehaviorContext<PaymentState, IPaymentValidationFailed> context)
    {
        var notificationDto = new RentalNotificationDto
        {
            Username = context.Message.Payment.Username,
            Body = $"Payment of {context.Message.Payment.Value}, please check your payment details",
            Title = "Payment failed",
            Data = new Dictionary<string, string>
            {
                {Event, "payment-failed"},
                {PaymentId, context.Message.Payment.Id.ToString()}
            },
            RentalId = context.Message.Payment.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }
}