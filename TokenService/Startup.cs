using dotnet_etcd;
using dotnet_etcd.interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;
using TokenService.Services;

namespace TokenService;

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
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "TokenService", Version = "v1"});
        });

        services.AddScoped<ITokenService, Services.TokenService>();
        services.AddSingleton<IEtcdClient, EtcdClient>(_ => new EtcdClient(Configuration.GetConnectionString("Etcd")));
        
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
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TokenService v1"));

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
