namespace Common.Models.Enums
{
    public enum RentalStatus
    {
        Unknown = 0,
        Submitted,
        BikeValidated,
        BikeValidationFailed,
        BikeReserved,
        BikeReservationFailed,
        BikeUnlocked,
        BikeUnlockFailed,
        BikeLocked,
        BikeLockFailed,
        BikeAttached,
        BikeAttachFailed
    }
}