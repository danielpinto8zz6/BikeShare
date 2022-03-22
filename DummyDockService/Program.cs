using AutoMapper;
using Common.Models;
using Common.Services.Repositories;
using DummyDockService.Consumers;
using DummyDockService.Models.Dtos;
using DummyDockService.Models.Entities;
using LSG.GenericCrud.Dto.Services;
using LSG.GenericCrud.Repositories;
using LSG.GenericCrud.Services;
using MassTransit;
using MongoDB.Driver;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddServiceDiscovery(opt => opt.UseEureka());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMongoClient, MongoClient>(_ =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));

builder.Services.AddScoped<ICrudRepository, MongoDbRepository>(provider =>
{
    var mongoClient = provider.GetRequiredService<IMongoClient>();

    return new MongoDbRepository(mongoClient, "dummy-dock");
});

builder.Services.AddScoped<ICrudService<Guid, DummyDockDto>, CrudServiceBase<Guid, DummyDockDto, DummyDock>>();
builder.Services.AddScoped<ICrudService<Guid, DummyDock>, CrudServiceBase<Guid, DummyDock>>();

var automapperConfiguration = new MapperConfiguration(conf =>
{
    conf.CreateMap<DummyDockDto, DummyDock>();
    conf.CreateMap<DummyDock, DummyDockDto>();
});

builder.Services.AddSingleton(automapperConfiguration.CreateMapper());

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BikeUnlockConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConfiguration =
            builder.Configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();

        cfg.Host(new Uri(rabbitMqConfiguration.Host), h =>
        {
            h.Username(rabbitMqConfiguration.Username);
            h.Password(rabbitMqConfiguration.Password);
        });

        cfg.ReceiveEndpoint("bike-unlock",
            e => { e.ConfigureConsumer<BikeUnlockConsumer>(context); });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddMassTransitHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();