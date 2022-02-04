using Common.Models.Dtos;
using LSG.GenericCrud.Controllers;
using LSG.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : CrudControllerBase<string, ApplicationUserDto>
    {
        public UsersController(ICrudService<string, ApplicationUserDto> service) : base(service)
        {
        }
    }
}