using System;
using System.Threading.Tasks;
using UserService.Models.Entities;
using LSG.GenericCrud.DataFillers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace UserService.DataFillers
{
    public class PasswordDataFiller : IEntityDataFiller
    {
        public bool IsEntitySupported(EntityEntry entry)
        {
            var result = entry.Entity is ApplicationUser &&
                         entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted;

            return result;
        }

        public Task<object> FillAsync(EntityEntry entry)
        {
            if (entry.State == EntityState.Added) ((ApplicationUser) entry.Entity).CreatedDate = DateTime.UtcNow;

            if (entry.State == EntityState.Deleted) ((ApplicationUser) entry.Entity).DeletedDate = DateTime.UtcNow;
            
            ((ApplicationUser) entry.Entity).ModifiedDate = DateTime.Now;

            return Task.FromResult(entry.Entity);
        }
    }
}