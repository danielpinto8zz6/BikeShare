using System;
using LSG.GenericCrud.Repositories;
using Microsoft.EntityFrameworkCore;
using UserService.Models.Entities;

namespace UserService.Data
{
    public class ApplicationDbContext : BaseDbContext, IDbContext
    {
        public ApplicationDbContext(DbContextOptions options, IServiceProvider serviceProvider) : base(options,
            serviceProvider)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; }
    }
}