namespace Common.Models.Dtos;

public class ApplicationUserDto : IEntity<string>
{
    public string Id { get; set; }

    public string Username { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }

    public string PasswordHash { get; set; }

    public List<CreditCardDto> CreditCards { get; set; }
}