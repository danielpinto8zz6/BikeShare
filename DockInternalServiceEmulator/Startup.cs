using AutoMapper;
using Common.Models;
using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using Common.Services.Repositories;
using DockInternalServiceEmulator.Consumers;
using DockInternalServiceEmulator.Services;
using dotnet_etcd;
using dotnet_etcd.interfaces;
using MassTransit;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;

namespace DockInternalServiceEmulator
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
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "BikeService", Version = "v1"});
            });
            
            services.AddSingleton<IEtcdClient, EtcdClient>(_ => new EtcdClient(Configuration.GetConnectionString("Etcd")));

            services.AddMassTransit(x =>
            {
                x.AddConsumer<UnlockBikeConsumer>();

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
                    
                    cfg.ReceiveEndpoint(nameof(IUnlockBike),
                        e => { e.ConfigureConsumer<UnlockBikeConsumer>(context); });
                });
            });

            services.AddScoped<IDockInternalService, DockInternalService>();
            
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
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BikeService v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.Map<HealthEndpoint>();
                endpoints.Map<InfoEndpoint>();
            });
        }
    }
}