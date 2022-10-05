using System;
using AutoMapper;
using Common.Models;
using Common.Models.Dtos;
using Common.Models.Events.Payment;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using PaymentService.Models.Entities;
using PaymentService.Repositories;
using PaymentService.Saga;
using PaymentService.Services;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;

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

            var mongoDbConnectionString = Configuration.GetConnectionString("MongoDb");
            Console.WriteLine(mongoDbConnectionString);
            services.AddScoped<IMongoClient, MongoClient>(_ =>
                new MongoClient(mongoDbConnectionString));

            services.AddScoped<IPaymentService, Services.PaymentService>();
            services.AddScoped<IPaymentRepository, PaymentRepository>(provider =>
            {
                var mongoClient = provider.GetRequiredService<IMongoClient>();

                return new PaymentRepository(mongoClient, "payment");
            });

            var automapperConfiguration = new MapperConfiguration(conf =>
            {
                conf.CreateMap<PaymentDto, Payment>();
                conf.CreateMap<Payment, PaymentDto>();
            });

            services.AddSingleton(automapperConfiguration.CreateMapper());

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

                    cfg.ReceiveEndpoint(nameof(IPaymentRequested), e => { e.ConfigureSaga<PaymentState>(context); });
                    cfg.ReceiveEndpoint(nameof(IPaymentCalculated), e => { e.ConfigureSaga<PaymentState>(context); });
                    cfg.ReceiveEndpoint(nameof(IPaymentValidated), e => { e.ConfigureSaga<PaymentState>(context); });
                    cfg.ReceiveEndpoint(nameof(IPaymentFailed),
                        e => { e.ConfigureSaga<PaymentState>(context); });

                    cfg.ConfigureEndpoints(context);
                });

                x.AddSagaStateMachine<PaymentStateMachine, PaymentState>()
                    .MongoDbRepository(r =>
                    {
                        r.Connection = mongoDbConnectionString;
                        r.DatabaseName = "sagas";
                        r.CollectionName = "PaymentSaga";
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
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentService v1"));

            app.UseHttpsRedirection();

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