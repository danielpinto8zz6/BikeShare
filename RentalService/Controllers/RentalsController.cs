using System;
using Common.Models.Dtos;
using LSG.GenericCrud.Controllers;
using Microsoft.AspNetCore.Mvc;
using RentalService.Services;

namespace RentalService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalsController : CrudControllerBase<Guid, RentalDto>
    {
        public RentalsController(IRentalService service) : base(service)
        {
        }
    }
}