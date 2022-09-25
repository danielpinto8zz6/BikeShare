using System;
using AutoMapper;
using Common.Models;
using Common.Models.Dtos;
using Common.Models.Events.Rental;
using Common.Services;
using Common.Services.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using RentalService.Models.Entities;
using RentalService.Saga;
using RentalService.Services;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;

namespace RentalService
{
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
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "RentalService", Version = "v1"});
            });

            services.AddScoped<IMongoClient, MongoClient>(_ =>
                new MongoClient(Configuration.GetConnectionString("MongoDb")));

            services.AddScoped<IRentalService, Services.RentalService>();
            services.AddScoped<IMongoDbRepository, MongoDbRepository>(provider =>
            {
                var mongoClient = provider.GetRequiredService<IMongoClient>();

                return new MongoDbRepository(mongoClient, "rental");
            });

            var automapperConfiguration = new MapperConfiguration(conf =>
            {
                conf.CreateMap<RentalDto, Rental>();
                conf.CreateMap<Rental, RentalDto>();
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
                
                x.AddSagaStateMachine<RentalStateMachine, RentalState>()
                    .MongoDbRepository(r =>
                    {
                        r.Connection = "mongodb://adminuser:password123@192.168.1.199:31000";
                        r.DatabaseName = "sagas";
                        r.CollectionName = "sagas";
                    });
            });

            services.AddTransient<IProducer<IRentalSubmitted>, Producer<IRentalSubmitted>>();

            services.AddSingleton(_ => new EndpointResolver(rabbitMqConfiguration.Host));
            
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
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RentalService v1"));

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
}