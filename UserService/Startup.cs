using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using LSG.GenericCrud.DataFillers;
using LSG.GenericCrud.Dto.Helpers;
using LSG.GenericCrud.Dto.Services;
using LSG.GenericCrud.Helpers;
using LSG.GenericCrud.Models;
using LSG.GenericCrud.Repositories;
using LSG.GenericCrud.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Steeltoe.Discovery.Client;
using UserService.Data;
using UserService.DataFillers;
using UserService.Models;
using UserService.Models.Dtos;
using UserService.Models.Entities;

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
            services.AddDiscoveryClient(Configuration);
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "UserService", Version = "v1"});
            });

            services
                .AddScoped<ICrudService<Guid, ApplicationUserDto>,
                    CrudServiceBase<Guid, ApplicationUserDto, ApplicationUser>>();

            services.AddTransient<IEntityDataFiller, PasswordDataFiller>();

            services.AddTransient<IDbContext, ApplicationDbContext>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql("Server=192.168.1.200;port=5432;Database=user;User Id=bikeshare;Password=bikeshare;"));

            // inject needed service and repository layers
            services.AddCrud();

            var automapperConfiguration = new AutoMapper.MapperConfiguration(conf =>
            {
                conf.CreateMap<ApplicationUserDto, ApplicationUser>();
                conf.CreateMap<ApplicationUser, ApplicationUserDto>();
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    
            app.InitializeDatabase<ApplicationDbContext>();
        }
    }
}