namespace Common.Models.Enums
{
    public enum PaymentStatus
    {
        Unknown = 0,
        Requested,
        Calculated,
        CalculationFailed,
        Validated,
        ValidationFailed
    }
}