using System;
using Common.Models.Events.Rental;
using MassTransit;
using RentalService.Saga;

namespace RentalService;

public class EndpointResolver
{
    private readonly string _baseUrl;

    public EndpointResolver(string baseUrl)
    {
        _baseUrl = baseUrl + "/";
    }

    public Uri BikeValidateEndpoint => new(_baseUrl + "bike-validate");

    public Uri BikeReservationEndpoint => new(_baseUrl + "bike-reservation");

    public Uri BikeUnlockEndpoint => new(_baseUrl + "bike-unlock");

    public Uri BikeLockEndpoint => new(_baseUrl + "bike-lock");

    public Uri PaymentRequestEndpoint => new(_baseUrl + "payment-request");
}