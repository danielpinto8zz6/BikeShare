using Common.Models.Dtos;

namespace DockService.Models.Dtos
{
    public class NearByDocksRequestDto
    {
        public CoordinatesDto? Coordinates { get; set; }

        public double Radius { get; set; }

        public DockStatus FilterStatus { get; set; }
    }

    public enum DockStatus
    {
        All = 0,
        WithBike,
        WithoutBike
    }
}