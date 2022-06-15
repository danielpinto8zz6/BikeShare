using AutoMapper;
using Common.Extensions.DataFillers;
using Common.Services.Repositories;
using FeedbackService.Models.Dtos;
using FeedbackService.Models.Entities;
using FeedbackService.Services;
using LSG.GenericCrud.DataFillers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

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
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "FeedbackService", Version = "v1"});
        });

        services.AddScoped<IMongoClient, MongoClient>(_ =>
            new MongoClient(Configuration.GetConnectionString("MongoDb")));

        services.AddScoped<IFeedbackService, Services.FeedbackService>();
        services.AddScoped<IMongoDbRepository, MongoDbRepository>(provider =>
        {
            var mongoClient = provider.GetRequiredService<IMongoClient>();

            return new MongoDbRepository(mongoClient, "feedback");
        });

        var automapperConfiguration = new MapperConfiguration(conf =>
        {
            conf.CreateMap<FeedbackDto, Feedback>();
            conf.CreateMap<Feedback, FeedbackDto>();
        });

        services.AddSingleton(automapperConfiguration.CreateMapper());

        services.AddTransient<IEntityDataFiller, DateDataFiller>();
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

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}