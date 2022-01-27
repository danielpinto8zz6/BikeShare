using System;
using AutoMapper;
using BikeService.Consumers;
using BikeService.Models.Dtos;
using BikeService.Models.Entities;
using BikeService.Repositories;
using BikeService.Services;
using Common;
using Common.DataFillers;
using Common.Models;
using Common.Models.Dtos;
using Common.Repositories;
using LSG.GenericCrud.DataFillers;
using LSG.GenericCrud.Dto.Services;
using LSG.GenericCrud.Helpers;
using LSG.GenericCrud.Repositories;
using LSG.GenericCrud.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using Steeltoe.Discovery.Client;

namespace BikeService
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
            services.AddDiscoveryClient(Configuration);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "BikeService", Version = "v1"});
            });

            services.AddScoped<ICrudService<Guid, BikeDto>, CrudServiceBase<Guid, BikeDto, Bike>>();
            services.AddScoped<ICrudService<Guid, Bike>, CrudServiceBase<Guid, Bike>>();
            services.AddScoped<IBikeService, Services.BikeService>();
            services.AddScoped<IBikeRepository, BikeRepository>(provider =>
            {
                var mongoClient = provider.GetRequiredService<IMongoClient>();

                return new BikeRepository(mongoClient, "bike");
            });

            services.AddCrud();

            var automapperConfiguration = new MapperConfiguration(conf =>
            {
                conf.CreateMap<BikeDto, Bike>()
                    .ForMember(item => item.Coordinates, expression => expression.MapFrom(src =>
                        new GeoJson2DGeographicCoordinates(src.Coordinates.Longitude, src.Coordinates.Latitude)));

                conf.CreateMap<Bike, BikeDto>()
                    .ForMember(item => item.Coordinates, expression => expression.MapFrom(src =>
                        new CoordinatesDto
                        {
                            Latitude = src.Coordinates.Latitude, Longitude = src.Coordinates.Longitude
                        }));
            });

            services.AddSingleton(automapperConfiguration.CreateMapper());

            services.AddTransient<IEntityDataFiller, DateDataFiller>();

            services.AddScoped<IMongoClient, MongoClient>(_ =>
                new MongoClient(Configuration.GetConnectionString("MongoDb")));

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
                        Configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();

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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BikeService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            // app.InitializeDatabase<ApplicationDbContext>();
        }
    }
}