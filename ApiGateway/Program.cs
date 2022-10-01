using System.Text;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Eureka;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

namespace AgentPortalApiGateway
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            var key = Encoding.ASCII.GetBytes("2e7a1e80-16ee-4e52-b5c6-5e8892453459");

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", true, false)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true,
                            false)
                        .AddJsonFile("ocelot.json", false, false)
                        .AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, false)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices(services =>
                {
                    services.AddServiceDiscovery(opt => opt.UseEureka());
                    services.AddCors();
                    services.AddAuthentication(x =>
                        {
                            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        })
                        .AddJwtBearer("ApiSecurity", x =>
                        {
                            x.RequireHttpsMetadata = false;
                            x.SaveToken = true;
                            x.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(key),
                                ValidateIssuer = false,
                                ValidateAudience = false
                            };
                        });

                    services.AddOcelot()
                        .AddEureka()
                        .AddCacheManager(x => x.WithDictionaryHandle());
                })
                .Configure(app =>
                {
                    var appSettings = new AppSettings();
                    app.ApplicationServices.GetService<IConfiguration>()
                        ?.GetSection("AppSettings")
                        .Bind(appSettings);

                    app.UseCors
                    (b => b
                        .WithOrigins(appSettings.AllowedChatOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                    );
                    app.UseMiddleware<RequestResponseLoggingMiddleware>();
                    app.UseOcelot().Wait();
                })
                .Build();
        }
    }
}