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
                   entry.State is EntityState.Added or EntityState.Modified;

            return result;
        }

        public async Task<object> FillAsync(EntityEntry entry)
        {
            if (entry.State == EntityState.Added) ((ApplicationUser) entry.Entity).CreatedDate = DateTime.Now;
            ((ApplicationUser) entry.Entity).ModifiedDate = DateTime.Now;
            ((ApplicationUser) entry.Entity).Password = $"{((ApplicationUser) entry.Entity).Password}1234";

            return (ApplicationUser) entry.Entity;
        }
    }
}