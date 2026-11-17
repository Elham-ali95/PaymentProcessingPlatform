
using Paymentprocess.Api.DTOs;
using Paymentprocess.Api.Interfaces;
using Paymentprocess.Api.Data;
using Microsoft.EntityFrameworkCore;
using Paymentprocess.Api.Models;
using Paymentprocess.Api.Enums;

namespace Paymentprocess.Api.Services
{
    public class PaymentService : IPayments
    {
        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
        {
            var customerExists = await _context.Customers.AnyAsync(c => c.ID == request.CustomerID);

            if (!customerExists)
            {
                throw new ArgumentException("Customer does not exist.");
            }

            if (request.OriginalPaymentAmount <= 0)
            {
                throw new ArgumentException("Payment amount must be greater than zero.");
            }

            var payment = new Payments
            {
                CustomerID = request.CustomerID,
                MerchantReference = request.MerchantReference,
                OriginalPaymentAmount = request.OriginalPaymentAmount,
                OutstandingAmount = request.OriginalPaymentAmount,
                Currency = request.Currency,
                PaymentStatus = PaymentStatus.Pending,
                CreationDate = DateTime.UtcNow
            };

            // 1. Create payment
            _context.Payments.Add(payment);

            // 2. Create payment transaction
            var transaction = new Transactions
            {
                ID = Guid.NewGuid(),
                PaymentID = payment.ID,
                TransactionType = TransactionType.PaymentCreated,
                Amount = payment.OriginalPaymentAmount,
                Reference = payment.MerchantReference,
                CreationDate = payment.CreationDate
            };

            _context.Transactions.Add(transaction);

            // 3. Save payment and transaction together
            await _context.SaveChangesAsync();

            var response = new CreatePaymentResponse
            {
                ID = payment.ID,
                CustomerID = payment.CustomerID,
                MerchantRefrence = payment.MerchantReference,
                OriginalPaymentAmount = payment.OriginalPaymentAmount,
                OutstandingAmount = payment.OutstandingAmount,
                Currency = payment.Currency,
                PaymentStatus = payment.PaymentStatus,
                CreationDate = payment.CreationDate
            };

            return response;
        }

        public async Task<CreatePaymentResponse?> GetPaymentByIdAsync(Guid paymentId)
        {
            var response = await _context.Payments
                .Where(p => p.ID == paymentId)
                .Select(p => new CreatePaymentResponse
                {
                    ID = p.ID,
                    CustomerID = p.CustomerID,
                    MerchantRefrence = p.MerchantReference,
                    OriginalPaymentAmount = p.OriginalPaymentAmount,
                    OutstandingAmount = p.OutstandingAmount,
                    Currency = p.Currency,
                    PaymentStatus = p.PaymentStatus,
                    CreationDate = p.CreationDate
                })
                .FirstOrDefaultAsync();

            return response;
        }

        public async Task<IEnumerable<CreatePaymentResponse>> GetPaymentsByCustomerIdAsync(Guid customerId)
        {
            var customerExists = await _context.Customers
                .AnyAsync(c => c.ID == customerId);

            if (!customerExists)
            {
                throw new ArgumentException("Customer does not exist.");
            }

            var payments = await _context.Payments
                .Where(p => p.CustomerID == customerId)
                .Select(p => new CreatePaymentResponse
                {
                    ID = p.ID,
                    CustomerID = p.CustomerID,
                    MerchantRefrence = p.MerchantReference,
                    OriginalPaymentAmount = p.OriginalPaymentAmount,
                    OutstandingAmount = p.OutstandingAmount,
                    Currency = p.Currency,
                    PaymentStatus = p.PaymentStatus,
                    CreationDate = p.CreationDate
                })
                .ToListAsync();

            return payments;
        }

        private readonly ApplicationDbContext _context;
    }
}
