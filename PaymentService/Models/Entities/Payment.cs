using System;
using Common.Models.Dtos;

namespace PaymentService.Models.Entities;

public class Payment : IBaseEntity
{
    public Guid Id { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime? ModifiedDate { get; set; }
    
    public DateTime? DeletedDate { get; set; }
}