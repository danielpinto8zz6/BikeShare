using System;
using FeedbackService.Models.Dtos;
using LSG.GenericCrud.Controllers;
using LSG.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;

namespace FeedbackService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbacksController : CrudControllerBase<Guid, FeedbackDto>
    {
        public FeedbacksController(ICrudService<Guid, FeedbackDto> service) : base(service)
        {
        }
    }
}