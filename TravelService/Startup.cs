using AutoMapper;
using Common.Models;
using Common.Models.Dtos;
using Common.Models.Events.Rental;
using Common.Services;
using Common.Services.Repositories;
using Common.TravelEvent.Entities;
using Common.TravelEvent.Repositories;
using Common.TravelEvent.Services;
using MassTransit;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;

namespace TravelService;

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
            services.AddServiceDiscovery(opt => opt.UseEureka());
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "TravelService", Version = "v1"});
            });

            var mongoDbConnectionString = Configuration.GetConnectionString("MongoDb");
            services.AddScoped<IMongoClient, MongoClient>(_ => new MongoClient(mongoDbConnectionString));

            services.AddScoped<ITravelEventService, TravelEventService>();
            services.AddScoped<ITravelEventRepository, TravelEventRepository>(provider =>
            {
                var mongoClient = provider.GetRequiredService<IMongoClient>();

                return new TravelEventRepository(mongoClient, "travel-event");
            });
            
            var automapperConfiguration = new MapperConfiguration(conf =>
            {
                conf.CreateMap<TravelEventDto, TravelEvent>();
                conf.CreateMap<TravelEvent, TravelEventDto>();
            });

            services.AddSingleton(automapperConfiguration.CreateMapper());

            var rabbitMqConfiguration = Configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();
            
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(rabbitMqConfiguration.Host), h =>
                    {
                        h.Username(rabbitMqConfiguration.Username);
                        h.Password(rabbitMqConfiguration.Password);
                    });
                    
                    cfg.ConfigureEndpoints(context);
                });
            });
            
            services.AddSingleton<IHealthCheckHandler, ScopedEurekaHealthCheckHandler>();
            
            services.AddHealthActuator(Configuration);
            services.AddInfoActuator(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelService v1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.Map<InfoEndpoint>();
                endpoints.Map<HealthEndpoint>();
            });
        }
    }
