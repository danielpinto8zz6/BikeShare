using AutoMapper;
using Common.DataFillers;
using Common.Models;
using Common.Models.Dtos;
using Common.Repositories;
using DockService.Consumers;
using DockService.Models.Dtos;
using DockService.Models.Entities;
using DockService.Repositories;
using DockService.Services;
using LSG.GenericCrud.DataFillers;
using LSG.GenericCrud.Dto.Services;
using LSG.GenericCrud.Helpers;
using LSG.GenericCrud.Repositories;
using LSG.GenericCrud.Services;
using MassTransit;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using Steeltoe.Discovery.Client;

namespace DockService;

public static class ServiceExtensions
{
    public static void SetupServices(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddDiscoveryClient(configuration);

        services.AddControllers();
        services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "BikeService", Version = "v1"}); });

        services.AddScoped<ICrudService<Guid, DockDto>, CrudServiceBase<Guid, DockDto, Dock>>();
        services.AddScoped<ICrudService<Guid, Dock>, CrudServiceBase<Guid, Dock>>();
        services.AddScoped<IDockService, Services.DockService>();
        services.AddScoped<IDockRepository, DockRepository>(provider =>
        {
            var mongoClient = provider.GetRequiredService<IMongoClient>();

            return new DockRepository(mongoClient, "dock");
        });

        services.AddCrud();

        var automapperConfiguration = new MapperConfiguration(conf =>
        {
            conf.CreateMap<DockDto, Dock>()
                .ForMember(item => item.Coordinates, expression => expression.MapFrom(src =>
                    new GeoJson2DGeographicCoordinates(src.Coordinates.Longitude, src.Coordinates.Latitude)));

            conf.CreateMap<Dock, DockDto>()
                .ForMember(item => item.Coordinates, expression => expression.MapFrom(src =>
                    new CoordinatesDto
                    {
                        Latitude = src.Coordinates.Latitude, Longitude = src.Coordinates.Longitude
                    }));
        });

        services.AddSingleton(automapperConfiguration.CreateMapper());

        services.AddTransient<IEntityDataFiller, DateDataFiller>();

        services.AddScoped<IMongoClient, MongoClient>(_ =>
            new MongoClient(configuration.GetConnectionString("MongoDb")));

        services.AddScoped<ICrudRepository, MongoDbRepository>(provider =>
        {
            var mongoClient = provider.GetRequiredService<IMongoClient>();

            return new MongoDbRepository(mongoClient, "bike");
        });

        services.AddMassTransit(x =>
        {
            x.AddConsumer<BikeReservationConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqConfiguration =
                    configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();

                cfg.Host(new Uri(rabbitMqConfiguration.Host), h =>
                {
                    h.Username(rabbitMqConfiguration.Username);
                    h.Password(rabbitMqConfiguration.Password);
                });

                cfg.ReceiveEndpoint("bike-reservation",
                    e => { e.ConfigureConsumer<BikeReservationConsumer>(context); });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddMassTransitHostedService();
    }
}