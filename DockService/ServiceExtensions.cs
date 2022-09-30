using AutoMapper;
using Common.Models;
using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using DockService.Consumers;
using DockService.Models.Dtos;
using DockService.Models.Entities;
using DockService.Repositories;
using DockService.Services;
using dotnet_etcd;
using dotnet_etcd.interfaces;
using MassTransit;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using Nominatim.API.Geocoders;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;

namespace DockService;

public static class ServiceExtensions
{
    public static void SetupServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "BikeService", Version = "v1"}); });

        services.AddScoped<IMongoClient, MongoClient>(_ =>
            new MongoClient(configuration.GetConnectionString("MongoDb")));

        services.AddScoped<IDockManagerService, DockManagerService>();
        services.AddScoped<IDockService, Services.DockService>();
        services.AddScoped<IDockRepository, DockRepository>(provider =>
        {
            var mongoClient = provider.GetRequiredService<IMongoClient>();

            return new DockRepository(mongoClient, "dock");
        });

        var automapperConfiguration = new MapperConfiguration(conf =>
        {
            conf.CreateMap<DockDto, Dock>()
                .ForMember(item => item.Coordinates, expression => expression.MapFrom(src =>
                    src.Coordinates != null
                        ? new GeoJson2DGeographicCoordinates(src.Coordinates.Longitude,
                            src.Coordinates.Latitude)
                        : null));

            conf.CreateMap<Dock, DockDto>()
                .ForMember(item => item.Coordinates, expression => expression.MapFrom(src =>
                    src.Coordinates != null
                        ? new CoordinatesDto
                        {
                            Latitude = src.Coordinates.Latitude,
                            Longitude = src.Coordinates.Longitude
                        }
                        : null));
        });

        services.AddSingleton(automapperConfiguration.CreateMapper());

        services.AddScoped<ReverseGeocoder>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<BikeUnlockConsumer>();
            x.AddConsumer<BikeLockConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqConfiguration =
                    configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();

                cfg.Host(new Uri(rabbitMqConfiguration.Host), h =>
                {
                    h.Username(rabbitMqConfiguration.Username);
                    h.Password(rabbitMqConfiguration.Password);
                });

                cfg.ConfigureEndpoints(context);

                cfg.ReceiveEndpoint(nameof(IUnlockBike),
                    e => { e.ConfigureConsumer<BikeUnlockConsumer>(context); });
                cfg.ReceiveEndpoint(nameof(BikeLockRequest),
                    e => { e.ConfigureConsumer<BikeLockConsumer>(context); });
            });
        });

        services.AddSingleton<IHealthCheckHandler, ScopedEurekaHealthCheckHandler>();

        services.AddHealthActuator(configuration);
        services.AddInfoActuator(configuration);

        services.AddSingleton<IEtcdClient, EtcdClient>(_ => new EtcdClient(configuration.GetConnectionString("Etcd")));
        
        services.Configure<MqttConfiguration>(configuration.GetSection("Mqtt"));
        services.AddScoped<IMqttPublisher, MqttPublisher>();
        services.AddHostedService<MqttSubscriber>();
    }
}