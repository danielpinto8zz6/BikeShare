using System;
using AutoMapper;
using Common.Extensions.DataFillers;
using Common.Models;
using Common.Models.Dtos;
using LSG.GenericCrud.DataFillers;
using LSG.GenericCrud.Dto.Services;
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
using PaymentService.Consumers;
using PaymentService.Data;
using PaymentService.Extensions;
using PaymentService.Models.Entities;
using PaymentService.Saga;
using PaymentService.Services;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

namespace PaymentService
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
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "PaymentService",
                    Version = "v1"
                });
            });
            
            services.AddScoped<ICrudService<Guid, PaymentDto>, CrudServiceBase<Guid, PaymentDto, Payment>>();

            services.AddTransient<IDbContext, ApplicationDbContext>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("PostgresqlConnection")));

            // inject needed service and repository layers
            services.AddCrud();

            var automapperConfiguration = new MapperConfiguration(conf =>
            {
                conf.CreateMap<PaymentDto, Payment>();
                conf.CreateMap<Payment, PaymentDto>();
            });

            services.AddSingleton(automapperConfiguration.CreateMapper());

            services.AddTransient<IEntityDataFiller, DateDataFiller>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<PaymentRequestConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqConfiguration =
                        Configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();

                    cfg.Host(new Uri(rabbitMqConfiguration.Host), h =>
                    {
                        h.Username(rabbitMqConfiguration.Username);
                        h.Password(rabbitMqConfiguration.Password);
                    });

                    cfg.ReceiveEndpoint("payment", e => { e.ConfigureSaga<PaymentState>(context); });
                    cfg.ReceiveEndpoint("payment-request", e => { e.ConfigureConsumer<PaymentRequestConsumer>(context); });

                    cfg.ConfigureEndpoints(context);
                });

                x.AddSagaStateMachine<PaymentStateMachine, PaymentState>()
                    .MongoDbRepository(r =>
                    {
                        r.Connection = "mongodb://adminuser:password123@192.168.1.199:31000";
                        r.DatabaseName = "sagas";
                        r.CollectionName = "sagas";
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentService v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.InitializeDatabase<ApplicationDbContext>();
        }
    }
}