using System;
using AuthService.Extensions;
using AuthService.Gateways;
using AuthService.Gateways.Clients;
using AuthService.Helpers;
using AuthService.Models;
using AuthService.Services;
using Common.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using Steeltoe.Common.Discovery;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

namespace AuthService
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

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            
            services.AddControllers();
            services.AddJwtAuthentication(appSettings);

            services.AddSingleton<IAuthService, Services.AuthService>();
            services.AddSingleton<IPasswordService, PasswordService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IJwtService, JwtService>();
            
            services.AddSingleton<IUserGateway, UserGateway>();

            var userServiceOptions = Configuration.GetSection("UserServiceOptions").Get<UserServiceOptions>();

            services.AddTransient<DiscoveryHttpMessageHandler>();

            services.AddRefitClient<IUserClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(userServiceOptions.BaseUrl))
                .AddHttpMessageHandler<DiscoveryHttpMessageHandler>();
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
            
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}