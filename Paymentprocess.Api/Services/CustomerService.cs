using Paymentprocess.Api.Data;
using Paymentprocess.Api.DTOs;
using Paymentprocess.Api.Interfaces;
using Paymentprocess.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Paymentprocess.Api.Services
{
    public class CustomerService : ICustomers
    {
        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request)
        {
            var Customer = new Customers
            {
                ID = Guid.NewGuid(),
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                Email = request.Email,
                CreationDate = DateTime.UtcNow
            };

            _context.Customers.Add(Customer);
            await _context.SaveChangesAsync();

            var response = new CreateCustomerResponse
            {
                ID = Customer.ID,
                Firstname = Customer.Firstname,
                Lastname = Customer.Lastname,
                Email = Customer.Email,
                CreationDate = Customer.CreationDate
            };

            return response;
        }

        public async Task<IEnumerable<CreateCustomerResponse>> GetAllCustomersAsync()
        {
            var customers = await _context.Customers.Select(c => new CreateCustomerResponse
            {
                ID = c.ID,
                Firstname = c.Firstname,
                Lastname = c.Lastname,
                Email = c.Email,
                CreationDate = c.CreationDate
            }).ToListAsync();

            return customers;
        }

        public async Task<CreateCustomerResponse?> GetCustomerByIdAsync(Guid customerId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ID == customerId);
            var response = customer != null ? new CreateCustomerResponse
            {
                ID = customer.ID,
                Firstname = customer.Firstname,
                Lastname = customer.Lastname,
                Email = customer.Email,
                CreationDate = customer.CreationDate
            } : null;

            return response;
        }
        

        private readonly ApplicationDbContext _context;
    }
}
