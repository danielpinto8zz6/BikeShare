using Common.Models.Dtos;

namespace DockService.Models.Dtos
{
    public class NearByDocksRequestDto
    {
        public CoordinatesDto Coordinates { get; set; }

        public double Radius { get; set; }

        public bool OnlyAvailable { get; set; }
    }
}