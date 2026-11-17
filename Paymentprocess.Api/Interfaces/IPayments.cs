using Paymentprocess.Api.DTOs;

namespace Paymentprocess.Api.Interfaces
{
    public interface IPayments
    {
        Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);

        Task<CreatePaymentResponse?> GetPaymentByIdAsync(Guid paymentId);

        Task<IEnumerable<CreatePaymentResponse>> GetPaymentsByCustomerIdAsync(Guid customerId);
    }
}
