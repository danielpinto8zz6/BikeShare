using System.Text;
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

namespace ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceDiscovery(options => options.UseEureka());

            services.AddCors();
            services.AddOcelot()
                .AddEureka()
                .AddCacheManager(x => { x.WithDictionaryHandle(); });

            var key = Encoding.ASCII.GetBytes("2e7a1e80-16ee-4e52-b5c6-5e8892453459");

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
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }
    }
}