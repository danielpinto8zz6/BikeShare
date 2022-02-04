using System;
using FeedbackService.Models.Entities;
using LSG.GenericCrud.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FeedbackService.Data
{
    public class ApplicationDbContext : BaseDbContext, IDbContext
    {
        public ApplicationDbContext(DbContextOptions options, IServiceProvider serviceProvider) : base(options,
            serviceProvider)
        {
        }

        public DbSet<Feedback> Feedbacks { get; set; }
    }
}