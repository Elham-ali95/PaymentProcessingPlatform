using Paymentprocess.Api.Data;
using Paymentprocess.Api.Interfaces;
using Paymentprocess.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Paymentprocess.Api.Services
{
    public class TransactionServices : ITransactions
    {
        public TransactionServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transactions>> GetTransactionsByPaymentIdAsync(Guid paymentId)
        {
            // Check whether the payment exists
            var paymentExists = await _context.Payments.AnyAsync(p => p.ID == paymentId);

            if (!paymentExists)
            {
                throw new ArgumentException($"Payment with ID {paymentId} not found.");
            }

            // Retrieve transactions in chronological order
            var transactions = await _context.Transactions
                .Where(t => t.PaymentID == paymentId)
                .OrderBy(t => t.CreationDate)
                .ToListAsync();

            return transactions;
        }

        private readonly ApplicationDbContext _context;
    }
}
