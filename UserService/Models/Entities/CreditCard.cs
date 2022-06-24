using System;

namespace UserService.Models.Entities;

public class CreditCard
{
    public string CardNumber { get; set; }
    public string CardHolderName { get; set; }
    public string CvvCode { get; set; }
    public string ExpiryDate { get; set; }
}