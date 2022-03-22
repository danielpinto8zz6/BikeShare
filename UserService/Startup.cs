using System.Text.Json.Serialization;
using AutoMapper;
using Common.Extensions.DataFillers;
using Common.Models.Dtos;
using Common.Services;
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
using Steeltoe.Discovery.Eureka;
using UserService.Data;
using UserService.Extensions;
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
            services.AddServiceDiscovery(opt => opt.UseEureka());
            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "UserService", Version = "v1"});
            });

            services    
                .AddScoped<ICrudService<string, ApplicationUserDto>,
                    CrudServiceBase<string, ApplicationUserDto, ApplicationUser>>();

            services.AddTransient<IEntityDataFiller, DateDataFiller>();

            services.AddTransient<IDbContext, ApplicationDbContext>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("PostgresqlConnection")));

            // inject needed service and repository layers
            services.AddCrud();

            var passwordService = new PasswordService();
            
            var automapperConfiguration = new MapperConfiguration(conf =>
            {
                conf.CreateMap<ApplicationUserDto, ApplicationUser>()
                    .ForMember(item => item.Id, expression => expression.MapFrom(src => src.Username))
                    .ForMember(item => item.PasswordHash, expression => expression.MapFrom(src => passwordService.Hash(src.Password)));

                conf.CreateMap<ApplicationUser, ApplicationUserDto>()
                    .ForMember(item => item.Password, opt => opt.Ignore());

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

            app.InitializeDatabase<ApplicationDbContext>();
        }
    }
}