using Microsoft.AspNetCore.Mvc;
using Paymentprocess.Api.DTOs;
using Paymentprocess.Api.Interfaces;

namespace Paymentprocess.Api.Controllers
{
    [ApiController]
    [Route("Api/Payments")]
    public class PaymentController : ControllerBase
    {
        public PaymentController(IPayments payments)
        {
            _payments = payments;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var payment = await _payments.CreatePaymentAsync(request);
            return CreatedAtAction(
                nameof(GetPaymentById),
                new { id = payment.ID },
                payment);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CreatePaymentResponse>> GetPaymentById(Guid id)
        {
            var payment = await _payments.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        [HttpGet("customer/{customerId:guid}")]
        public async Task<ActionResult<IEnumerable<CreatePaymentResponse>>> GetPaymentByCustomerId(Guid customerId)
        {
            var payment = await _payments.GetPaymentsByCustomerIdAsync(customerId);

            return Ok(payment);
        }
     

        private readonly IPayments _payments; 
    }
}
