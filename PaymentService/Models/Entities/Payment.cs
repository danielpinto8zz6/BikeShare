using System;
using Common.Models.Dtos;
using Common.Models.Enums;

namespace PaymentService.Models.Entities;

public class Payment : IBaseEntity<Guid>
{
    public Guid Id { get; set; }
    
    public string Username { get; set; }
        
    public PaymentStatus Status { get; set; }

    public double? Value { get; set; }
    
    public double? Duration { get; set; }

    public Guid RentalId { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime? ModifiedDate { get; set; }
    
    public DateTime? DeletedDate { get; set; }
}