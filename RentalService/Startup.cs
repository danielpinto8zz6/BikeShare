using System;
using AutoMapper;
using Common.Extensions.DataFillers;
using Common.Models;
using Common.Models.Dtos;
using Common.Models.Events;
using Common.Services;
using LSG.GenericCrud.DataFillers;
using LSG.GenericCrud.Helpers;
using LSG.GenericCrud.Repositories;
using LSG.GenericCrud.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RentalService.Data;
using RentalService.Extensions;
using RentalService.Models.Entities;
using RentalService.Services;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

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

            services.AddScoped<ICrudService<Guid, Rental>, CrudServiceBase<Guid, Rental>>();
            services.AddScoped<IRentalService, Services.RentalService>();

            services.AddTransient<IDbContext, ApplicationDbContext>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("PostgresqlConnection")));

            // inject needed service and repository layers
            services.AddCrud();

            var automapperConfiguration = new MapperConfiguration(conf =>
            {
                conf.CreateMap<RentalDto, Rental>();
                conf.CreateMap<Rental, RentalDto>();
            });

            services.AddSingleton(automapperConfiguration.CreateMapper());

            services.AddTransient<IEntityDataFiller, DateDataFiller>();

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

            services.AddScoped<IProducer<IRentalSubmitted>, Producer<IRentalSubmitted>>();
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

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.InitializeDatabase<ApplicationDbContext>();
        }
    }
}