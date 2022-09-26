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
        
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> CreateAsync(
            [FromBody] PaymentDto paymentDto)
        {
            var result = await _service.CreateAsync(paymentDto);

            return CreatedAtAction("GetById", new
            {
                id = result.Id
            }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            Guid id,
            [FromBody] PaymentDto paymentDto)
        {
            try
            {
                await _service.UpdateAsync(id, paymentDto);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}