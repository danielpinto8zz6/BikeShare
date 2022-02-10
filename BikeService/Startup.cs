using System;
using AutoMapper;
using BikeService.Data;
using BikeService.Models.Entities;
using Common;
using Common.DataFillers;
using Common.Models.Dtos;
using LSG.GenericCrud.DataFillers;
using LSG.GenericCrud.Dto.Services;
using LSG.GenericCrud.Helpers;
using LSG.GenericCrud.Repositories;
using LSG.GenericCrud.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Steeltoe.Discovery.Client;

namespace BikeService
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
            services.AddDiscoveryClient(Configuration);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "BikeService", Version = "v1"});
            });
            
            services
                .AddScoped<ICrudService<Guid, BikeDto>,
                    CrudServiceBase<Guid, BikeDto, BikeDto>>();

            services.AddTransient<IEntityDataFiller, DateDataFiller>();

            services.AddTransient<IDbContext, ApplicationDbContext>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("PostgresqlConnection")));

            // inject needed service and repository layers
            services.AddCrud();

            var automapperConfiguration = new MapperConfiguration(conf =>
            {
                conf.CreateMap<BikeDto, Bike>();
                conf.CreateMap<Bike, BikeDto>();
            });

            services.AddSingleton(automapperConfiguration.CreateMapper());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BikeService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.InitializeDatabase<ApplicationDbContext>();
        }
    }
}