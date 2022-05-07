using System;
using LSG.GenericCrud.Repositories;
using Microsoft.EntityFrameworkCore;
using PaymentService.Models.Entities;

namespace PaymentService.Data
{
    public class ApplicationDbContext : BaseDbContext, IDbContext
    {
        public ApplicationDbContext(DbContextOptions options, IServiceProvider serviceProvider) : base(options,
            serviceProvider)
        {
        }

        public DbSet<Payment> Payments { get; set; }
    }
}