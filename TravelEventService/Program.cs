using AutoMapper;
using Common.Models;
using Common.Models.Constants;
using Common.Models.Dtos;
using Common.TravelEvent.Entities;
using Common.TravelEvent.Repositories;
using Common.TravelEvent.Services;
using MassTransit;
using MongoDB.Driver;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;
using TravelEventService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDiscovery(opt => opt.UseEureka());

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TravelEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConfiguration =
            builder.Configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();

        cfg.Host(new Uri(rabbitMqConfiguration.Host), h =>
        {
            h.Username(rabbitMqConfiguration.Username);
            h.Password(rabbitMqConfiguration.Password);
        });

        cfg.ReceiveEndpoint(QueueConstants.TravelEventQueue,
            e => { e.ConfigureConsumer<TravelEventConsumer>(context); });

        cfg.ConfigureEndpoints(context);
    });
});

var automapperConfiguration = new MapperConfiguration(conf =>
{
    conf.CreateMap<TravelEventDto, TravelEvent>();
    conf.CreateMap<TravelEvent, TravelEventDto>();
});

builder.Services.AddSingleton(automapperConfiguration.CreateMapper());

builder.Services.AddScoped<IMongoClient, MongoClient>(_ =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));

builder.Services.AddScoped<ITravelEventService, Common.TravelEvent.Services.TravelEventService>();
builder.Services.AddScoped<ITravelEventRepository, TravelEventRepository>(provider =>
{
    var mongoClient = provider.GetRequiredService<IMongoClient>();

    return new TravelEventRepository(mongoClient, "travel-event");
});

builder.Services.AddSingleton<IHealthCheckHandler, ScopedEurekaHealthCheckHandler>();

builder.Services.AddHealthActuator(builder.Configuration);
builder.Services.AddInfoActuator(builder.Configuration);

var app = builder.Build();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.Map<InfoEndpoint>();
    endpoints.Map<HealthEndpoint>();
});

app.Run();