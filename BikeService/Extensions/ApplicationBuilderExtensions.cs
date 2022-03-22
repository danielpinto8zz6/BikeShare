using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BikeService.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void InitializeDatabase<T>(this IApplicationBuilder app) where T : DbContext
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
            var context = serviceScope?.ServiceProvider.GetRequiredService<T>();
            context?.Database.EnsureCreated();
        }
    }
}