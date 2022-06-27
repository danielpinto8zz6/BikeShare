namespace Common.Models.Events.Rental
{
    public interface IRentalFailure : IRentalMessage
    {
        public string ErrorMessage { get; set; }
    }
}