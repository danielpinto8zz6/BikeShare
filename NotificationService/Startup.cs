using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Models;
using Common.Models.Dtos;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NotificationService.Consumers;
using NotificationService.Gateways;
using NotificationService.Gateways.Client;
using NotificationService.Services;
using Refit;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

namespace NotificationService
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
            services.AddServiceDiscovery(options => options.UseEureka());

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "NotificationService", Version = "v1"});
            });

            services.AddMassTransit(x =>
            {
                x.AddConsumer<NotificationConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqConfiguration =
                        Configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();

                    cfg.Host(new Uri(rabbitMqConfiguration.Host), h =>
                    {
                        h.Username(rabbitMqConfiguration.Username);
                        h.Password(rabbitMqConfiguration.Password);
                    });

                    cfg.ReceiveEndpoint("notification",
                        e => { e.ConfigureConsumer<NotificationConsumer>(context); });

                    cfg.ConfigureEndpoints(context);
                });
            });
            
            var tokenServiceOptions = Configuration.GetSection("TokenService").Get<ServiceOptions>();

            services.AddTransient<DiscoveryHttpMessageHandler>();

            services.AddRefitClient<ITokenClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(tokenServiceOptions.BaseUrl))
                .AddHttpMessageHandler<DiscoveryHttpMessageHandler>();
            
            services.AddSingleton<ITokenGateway, TokenGateway>();
            
            services.AddSingleton<IMobileMessagingClient, MobileMessagingClient>();
            services.AddSingleton<INotificationService, Services.NotificationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotificationService v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}