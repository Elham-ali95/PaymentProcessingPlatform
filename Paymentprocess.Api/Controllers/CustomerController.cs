using Microsoft.AspNetCore.Mvc;
using Paymentprocess.Api.DTOs;
using Paymentprocess.Api.Interfaces;
using Paymentprocess.Api.Models;

namespace Paymentprocess.Api.Controllers
{
    [ApiController]
    [Route("api/Customers")]
    public class CustomerController : ControllerBase
    {
        public CustomerController(ICustomers customers)
        {
            _customers = customers;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            var customer = await _customers.CreateCustomerAsync(request);
            return CreatedAtAction(
                nameof(GetCustomerById),
                new { id = customer.ID },
                customer);

        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CreateCustomerResponse>> GetCustomerById(Guid id)
        {
            var customer = await _customers.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CreateCustomerResponse>>> GetAllCustomers()
        {
            var customers = await _customers.GetAllCustomersAsync();
            return Ok(customers);
        }

        private readonly ICustomers _customers;
    }
}
