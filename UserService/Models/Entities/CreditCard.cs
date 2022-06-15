using System;

namespace UserService.Models.Entities;

public class CreditCard
{
    public string Number { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public DateTime Validity { get; set; }
}