namespace Common.Models.Events.Rental
{
    public interface IRentalFailed : IRentalMessage
    {
        public string ErrorMessage { get; set; }
    }
}