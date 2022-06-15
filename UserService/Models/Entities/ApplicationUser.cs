using System;
using System.Collections.Generic;
using Common.Models.Dtos;

namespace UserService.Models.Entities
{
    public class ApplicationUser : IEntity<string>, IDateEntity
    {
        public string Id { get; set; }
        
        public string Username { get; set; }
        
        public string Name { get; set; }

        public string PasswordHash { get; set; }

        public List<CreditCard> CreditCards { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public DateTime? DeletedDate { get; set; }
    }
}