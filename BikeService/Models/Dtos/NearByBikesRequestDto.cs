using Common.Models.Dtos;

namespace BikeService.Models.Dtos
{
    public class NearByBikesRequestDto
    {
        public CoordinatesDto Coordinates { get; set; }

        public double Radius { get; set; }

        public bool OnlyAvailable { get; set; }
    }
}