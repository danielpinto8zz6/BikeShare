using System;
using System.Threading.Tasks;
using Common.Extensions.Exceptions;
using Common.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Services;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentsController(IPaymentService service)
        {
            _service = service;
        }

        [HttpGet("rental/{rentalId}")]
        public async Task<ActionResult<PaymentDto>> GetByRentalIdAsync(Guid rentalId)
        {
            try
            {
                var result = await _service.GetByRentalIdAsync(rentalId);
                return Ok(result);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                return Ok(result);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}