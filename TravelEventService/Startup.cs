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

namespace TravelEventService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddServiceDiscovery(opt => opt.UseEureka());

        services.AddMassTransit(x =>
        {
            x.AddConsumer<TravelEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqConfiguration =
                    Configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();

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

        services.AddSingleton(automapperConfiguration.CreateMapper());

        services.AddScoped<IMongoClient, MongoClient>(_ =>
            new MongoClient(Configuration.GetConnectionString("MongoDb")));

        services.AddScoped<ITravelEventService, Common.TravelEvent.Services.TravelEventService>();
        services.AddScoped<ITravelEventRepository, TravelEventRepository>(provider =>
        {
            var mongoClient = provider.GetRequiredService<IMongoClient>();

            return new TravelEventRepository(mongoClient, "travel-event");
        });

        services.AddSingleton<IHealthCheckHandler, ScopedEurekaHealthCheckHandler>();

        services.AddHealthActuator(Configuration);
        services.AddInfoActuator(Configuration);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseAuthentication();
        app.UseAuthentication();
        
        app.UseHttpsRedirection();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.Map<InfoEndpoint>();
            endpoints.Map<HealthEndpoint>();
        });
    }
}