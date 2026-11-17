using Microsoft.AspNetCore.Mvc;
using Paymentprocess.Api.Interfaces;

namespace Paymentprocess.Api.Controllers
{
    [Route("api/payments/{paymentId:guid}/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        public TransactionController(ITransactions transactions)
        {
            _transactions = transactions;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionsByPaymentId(Guid paymentId)
        {
            try
            {
                var transactions = await _transactions.GetTransactionsByPaymentIdAsync(paymentId);

                return Ok(transactions);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        private readonly ITransactions _transactions;
    }
}
