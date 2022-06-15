using System;
using AutoMapper;
using Common.Models;
using Common.Models.Dtos;
using Common.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Steeltoe.Discovery.Client;
using TravelEventService.Entities;

namespace TravelService
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
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "TravelService", Version = "v1"});
            });

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqConfiguration =
                        Configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();

                    cfg.Host(new Uri(rabbitMqConfiguration.Host), h =>
                    {
                        h.Username(rabbitMqConfiguration.Username);
                        h.Password(rabbitMqConfiguration.Password);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddScoped<IProducer<TravelEventDto>, Producer<TravelEventDto>>();

            var automapperConfiguration = new MapperConfiguration(conf =>
            {
                conf.CreateMap<TravelEventDto, TravelEvent>();
                conf.CreateMap<TravelEvent, TravelEventDto>();
            });

            services.AddSingleton(automapperConfiguration.CreateMapper());
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}