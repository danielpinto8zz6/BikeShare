using System;
using Common;
using LSG.GenericCrud.Models;

namespace UserService.Models.Entities
{
    public class ApplicationUser : IEntity<string>, IDateEntity
    {
        public string Id { get; set; }
        
        public string Username { get; set; }

        public string PasswordHash { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public DateTime? DeletedDate { get; set; }
    }
}