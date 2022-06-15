using AutoMapper;
using Common.Models;
using Common.Models.Constants;
using Common.Models.Dtos;
using Common.Services.Repositories;
using LSG.GenericCrud.Repositories;
using LSG.GenericCrud.Services;
using MassTransit;
using MongoDB.Driver;
using TravelEventService.Consumers;
using TravelEventService.Entities;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<ICrudRepository, MongoDbRepository>(provider =>
{
    var mongoClient = provider.GetRequiredService<IMongoClient>();

    return new MongoDbRepository(mongoClient, "travel-event");
});

builder.Services
    .AddScoped<ICrudService<Guid, TravelEvent>, CrudServiceBase<Guid, TravelEvent>>();

var app = builder.Build();

app.Run();