using System;
using BikeService.Models.Dtos;
using LSG.GenericCrud.Controllers;
using LSG.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BikesController : CrudControllerBase<Guid, BikeDto>
    {
        public BikesController(ICrudService<Guid, BikeDto> service) : base(service)
        {
        }
    }
}