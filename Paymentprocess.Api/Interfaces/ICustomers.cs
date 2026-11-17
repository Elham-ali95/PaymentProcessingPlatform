using Paymentprocess.Api.DTOs;
using Paymentprocess.Api.Models;

namespace Paymentprocess.Api.Interfaces
{
    public interface ICustomers
    {

        Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request);

        Task<IEnumerable<CreateCustomerResponse>> GetAllCustomersAsync();

        Task<CreateCustomerResponse?> GetCustomerByIdAsync(Guid customerId);
    }
}
