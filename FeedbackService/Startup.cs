using AutoMapper;
using Common.Services.Repositories;
using FeedbackService.Models.Dtos;
using FeedbackService.Models.Entities;
using FeedbackService.Repositories;
using FeedbackService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;

namespace FeedbackService;

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
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "FeedbackService", Version = "v1"});
        });

        services.AddScoped<IMongoClient, MongoClient>(_ =>
            new MongoClient(Configuration.GetConnectionString("MongoDb")));

        services.AddScoped<IFeedbackService, Services.FeedbackService>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>(provider =>
        {
            var mongoClient = provider.GetRequiredService<IMongoClient>();

            return new FeedbackRepository(mongoClient, "feedback");
        });

        var automapperConfiguration = new MapperConfiguration(conf =>
        {
            conf.CreateMap<FeedbackDto, Feedback>();
            conf.CreateMap<Feedback, FeedbackDto>();
        });

        services.AddSingleton(automapperConfiguration.CreateMapper());
        
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
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FeedbackService v1"));

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