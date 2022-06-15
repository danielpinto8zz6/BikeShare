namespace Common.Models.Dtos;

public class CreditCardDto
{
    public string Number { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public DateTime Validity { get; set; }
}