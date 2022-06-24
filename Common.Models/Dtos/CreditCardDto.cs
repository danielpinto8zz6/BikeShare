namespace Common.Models.Dtos;

public class CreditCardDto
{
    public string CardNumber { get; set; }
    public string CardHolderName { get; set; }
    public string CvvCode { get; set; }
    public string ExpiryDate { get; set; }
}
