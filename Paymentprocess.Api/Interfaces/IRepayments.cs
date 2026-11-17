using Paymentprocess.Api.DTOs;

namespace Paymentprocess.Api.Interfaces
{
    public interface IRepayments
    {
        Task<CreateRepaymentResponse> CreateRepaymentAsync(Guid paymentId, CreateRepaymentRequest request, string idempotencyKey);

        Task<CreateRepaymentResponse?> GetRepaymentByIdAsync(Guid repaymentId);

        Task<IEnumerable<CreateRepaymentResponse>> GetRepaymentsByPaymentIdAsync(Guid paymentId);
    }
}
