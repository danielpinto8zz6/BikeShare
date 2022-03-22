using System.Threading.Tasks;
using Common.Models.Dtos;

namespace BikeValidateService.Services
{
    public interface IBikeValidateService
    {
        Task<bool> IsBikeValidAsync(RentalDto rentalDto);
    }
}