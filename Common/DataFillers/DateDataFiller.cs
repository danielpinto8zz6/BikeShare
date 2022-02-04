using System;
using System.Threading.Tasks;
using LSG.GenericCrud.DataFillers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Common.DataFillers
{
    public class DateDataFiller : IEntityDataFiller
    {
        public bool IsEntitySupported(EntityEntry entry)
        {
            var result = entry.Entity is IDateEntity &&
                         entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted;

            return result;
        }

        public Task<object> FillAsync(EntityEntry entry)
        {
            if (entry.State == EntityState.Added) ((IDateEntity) entry.Entity).CreatedDate = DateTime.UtcNow;

            if (entry.State == EntityState.Deleted) ((IDateEntity) entry.Entity).DeletedDate = DateTime.UtcNow;

            ((IDateEntity) entry.Entity).ModifiedDate = DateTime.UtcNow;

            return Task.FromResult(entry.Entity);
        }
    }
}