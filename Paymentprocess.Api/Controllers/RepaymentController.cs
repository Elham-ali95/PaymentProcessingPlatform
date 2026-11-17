using Microsoft.AspNetCore.Mvc;
using Paymentprocess.Api.DTOs;
using Paymentprocess.Api.Interfaces;

namespace Paymentprocess.Api.Controllers
{
    [ApiController]
    [Route("api/Repayments")]
    public class RepaymentController : ControllerBase
    {
        public RepaymentController(IRepayments repayments)
        {
            _repayments = repayments;
        }

        [HttpPost("/api/payments/{paymentId:guid}/repayments")]
        public async Task<IActionResult> CreateRepayment(Guid paymentId,[FromBody] CreateRepaymentRequest request,
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
        {
            var repayment = await _repayments.CreateRepaymentAsync(
                paymentId,
                request,
                idempotencyKey);

            return CreatedAtAction(
                nameof(GetRepaymentById),
                new { repaymentId = repayment.ID },
                repayment);
        }

        [HttpGet("{repaymentId:guid}")]
        public async Task<ActionResult<CreateRepaymentResponse>> GetRepaymentById(
            Guid repaymentId)
        {
            var repayment = await _repayments.GetRepaymentByIdAsync(repaymentId);

            if (repayment == null)
            {
                return NotFound();
            }

            return Ok(repayment);
        }

        [HttpGet("/api/payments/{paymentId:guid}/repayments")]
        public async Task<ActionResult<IEnumerable<CreateRepaymentResponse>>>
            GetRepaymentsByPaymentId(Guid paymentId)
        {
            var repayments = await _repayments.GetRepaymentsByPaymentIdAsync(paymentId);

            return Ok(repayments);
        }

        private readonly IRepayments _repayments;
    }
}
