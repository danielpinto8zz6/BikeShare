using System;
using LSG.GenericCrud.Controllers;
using LSG.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;
using UserService.Models.Dtos;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : CrudControllerBase<Guid, ApplicationUserDto>
    {
        public UsersController(ICrudService<Guid, ApplicationUserDto> service) : base(service)
        {
        }
    }
}