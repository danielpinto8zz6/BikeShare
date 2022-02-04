using System;
using LSG.GenericCrud.Repositories;
using Microsoft.EntityFrameworkCore;
using RentalService.Models.Entities;

namespace RentalService.Data
{
    public class ApplicationDbContext : BaseDbContext, IDbContext
    {
        public ApplicationDbContext(DbContextOptions options, IServiceProvider serviceProvider) : base(options,
            serviceProvider)
        {
        }

        public DbSet<Rental> Rentals { get; set; }
    }
}