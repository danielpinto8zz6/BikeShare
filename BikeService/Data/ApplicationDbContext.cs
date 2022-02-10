using System;
using BikeService.Models.Entities;
using LSG.GenericCrud.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Data
{
    public class ApplicationDbContext : BaseDbContext, IDbContext
    {
        public ApplicationDbContext(DbContextOptions options, IServiceProvider serviceProvider) : base(options,
            serviceProvider)
        {
        }

        public DbSet<Bike> Bikes { get; set; }
    }
}