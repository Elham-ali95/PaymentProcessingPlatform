using Paymentprocess.Api.Models;

namespace Paymentprocess.Api.Interfaces
{
    public interface ITransactions
    {
        Task<IEnumerable<Transactions>> GetTransactionsByPaymentIdAsync(Guid paymentId);
    }
}
