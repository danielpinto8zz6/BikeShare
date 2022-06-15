namespace Common.Models.Dtos;

public class PaymentRequestDto
{
    public Guid RentalId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Username { get; set; }
}