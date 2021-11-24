using System;
using Common;

namespace UserService.Models.Dtos
{
    public class ApplicationUserDto : IBaseEntity
    {
        public Guid Id { get; set; }
    
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public DateTime? DeletedDate { get; set; }
    }
}