using System;
using LSG.GenericCrud.Controllers;
using LSG.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;
using RentalService.Models.Dto;

namespace RentalService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalsController : CrudControllerBase<Guid, RentalDto>
    {
        public RentalsController(ICrudService<Guid, RentalDto> service) : base(service)
        {
        }
    }
}