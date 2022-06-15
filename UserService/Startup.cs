using System.Text.Json.Serialization;
using AutoMapper;
using Common.Models.Dtos;
using Common.Services;
using Common.Services.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using UserService.Models.Entities;
using UserService.Services;

namespace UserService
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
            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "UserService", Version = "v1"});
            });

            services.AddScoped<IMongoClient, MongoClient>(_ =>
                new MongoClient(Configuration.GetConnectionString("MongoDb")));

            services.AddScoped<IUserService, Services.UserService>();
            services.AddScoped<IMongoDbRepository, MongoDbRepository>(provider =>
            {
                var mongoClient = provider.GetRequiredService<IMongoClient>();

                return new MongoDbRepository(mongoClient, "user");
            });

            var passwordService = new PasswordService();
            
            var automapperConfiguration = new MapperConfiguration(conf =>
            {
                conf.CreateMap<ApplicationUserDto, ApplicationUser>()
                    .ForMember(item => item.Id, expression => expression.MapFrom(src => src.Username))
                    .ForMember(item => item.PasswordHash, expression => expression.MapFrom(src => passwordService.Hash(src.Password)));

                conf.CreateMap<ApplicationUser, ApplicationUserDto>()
                    .ForMember(item => item.Password, opt => opt.Ignore());

                conf.CreateMap<CreditCard, CreditCardDto>();
                conf.CreateMap<CreditCardDto, CreditCard>();
            });

            services.AddSingleton(automapperConfiguration.CreateMapper());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}