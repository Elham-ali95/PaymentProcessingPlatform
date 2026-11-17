using Microsoft.EntityFrameworkCore;
using Paymentprocess.Api.Data;
using Paymentprocess.Api.DTOs;
using Paymentprocess.Api.Enums;
using Paymentprocess.Api.Interfaces;
using Paymentprocess.Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace Paymentprocess.Api.Services
{
    public class RepaymentService : IRepayments
    {
        public RepaymentService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<CreateRepaymentResponse> CreateRepaymentAsync(Guid paymentId, CreateRepaymentRequest request, string idempotencyKey)
        {
            // 1. Validate idempotency key
            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                throw new ArgumentException("Idempotency-Key header is required.");
            }

            // 2. Generate request hash
            var requestData = $"{paymentId}|{request.RepaymentAmount}|{request.ExternalReference}";

            var requestHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(requestData)));

            // 3. Check whether this request has already been processed
            var existingRecord = await _context.IdempotencyRecords
                .FirstOrDefaultAsync(r => r.IdempotencyKey == idempotencyKey);

            if (existingRecord != null)
            {
                if (existingRecord.RequestHash != requestHash)
                {
                    throw new InvalidOperationException(
                        "This Idempotency-Key has already been used with a different request.");
                }

                var existingRepayment = await _context.Repayments
                    .FirstOrDefaultAsync(r => r.ID == existingRecord.RepaymentID);

                if (existingRepayment == null)
                {
                    throw new InvalidOperationException(
                        "The original repayment associated with this Idempotency-Key was not found.");
                }

                return new CreateRepaymentResponse
                {
                    ID = existingRepayment.ID,
                    PaymentIdentifier = existingRepayment.PaymentIdentifier,
                    RepaymentAmount = existingRepayment.RepaymentAmount,
                    ExternalReference = existingRepayment.ExternalRefrence,
                    RepaymentStatus = existingRepayment.RepaymentStatus,
                    CreationDate = existingRepayment.CreationDate
                };
            }

            // 4. Retrieve payment
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.ID == paymentId);

            if (payment == null)
            {
                throw new ArgumentException($"Payment with ID {paymentId} not found.");
            }

            // 5. Validate payment status
            if (payment.PaymentStatus == PaymentStatus.Cancelled)
            {
                throw new InvalidOperationException(
                    "Cannot repay a cancelled payment.");
            }

            if (payment.PaymentStatus == PaymentStatus.Paid)
            {
                throw new InvalidOperationException(
                    "Payment is already fully paid.");
            }

            // 6. Validate repayment amount
            if (request.RepaymentAmount <= 0)
            {
                throw new ArgumentException(
                    "Repayment amount must be greater than zero.");
            }

            if (request.RepaymentAmount > payment.OutstandingAmount)
            {
                throw new ArgumentException(
                    "Repayment amount exceeds outstanding amount.");
            }

            // 7. Create repayment
            var repayment = new Repayment
            {
                ID = Guid.NewGuid(),
                PaymentIdentifier = paymentId,
                RepaymentAmount = request.RepaymentAmount,
                ExternalRefrence = request.ExternalReference,
                RepaymentStatus = RepaymentStatus.Completed,
                CreationDate = DateTime.UtcNow
            };

            // 8. Update payment balance and status
            payment.OutstandingAmount -= request.RepaymentAmount;

            payment.PaymentStatus = payment.OutstandingAmount == 0
                ? PaymentStatus.Paid
                : PaymentStatus.Active;

            // 9. Create transaction history record
            var transaction = new Transactions
            {
                ID = Guid.NewGuid(),
                PaymentID = paymentId,
                TransactionType = TransactionType.RepaymentReceived,
                Amount = request.RepaymentAmount,
                Reference = request.ExternalReference,
                CreationDate = repayment.CreationDate
            };

            // 10. Create idempotency record
            var idempotencyRecord = new IdempotencyRecord
            {
                ID = Guid.NewGuid(),
                IdempotencyKey = idempotencyKey,
                RequestHash = requestHash,
                RepaymentID = repayment.ID,
                CreationDate = DateTime.UtcNow
            };

            // 11. Save changes
            _context.Repayments.Add(repayment);
            _context.Transactions.Add(transaction);
            _context.IdempotencyRecords.Add(idempotencyRecord);

            await _context.SaveChangesAsync();

            // 12. Return response
            var response = new CreateRepaymentResponse
            {
                ID = repayment.ID,
                PaymentIdentifier = repayment.PaymentIdentifier,
                RepaymentAmount = repayment.RepaymentAmount,
                ExternalReference = repayment.ExternalRefrence,
                RepaymentStatus = repayment.RepaymentStatus,
                CreationDate = repayment.CreationDate
            };

            return response;
        }

        public async Task<CreateRepaymentResponse?> GetRepaymentByIdAsync(Guid repaymentId)
        {
            var repayment = await _context.Repayments.FirstOrDefaultAsync(r => r.ID == repaymentId);

            if (repayment == null)
            {
                throw new ArgumentException("repayment is not exist");
            }
            var response = new CreateRepaymentResponse
            {
                ID = repayment.ID,
                PaymentIdentifier = repayment.PaymentIdentifier,
                RepaymentAmount = repayment.RepaymentAmount,
                ExternalReference = repayment.ExternalRefrence,
                RepaymentStatus = repayment.RepaymentStatus,
                CreationDate = repayment.CreationDate

            };

            return response;
        }

        public async Task<IEnumerable<CreateRepaymentResponse>> GetRepaymentsByPaymentIdAsync(Guid paymentId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.ID == paymentId);

            if (payment == null)
            {
                throw new ArgumentException($"Payment with ID {paymentId} not found.");
            }

            var repayments = await _context.Repayments.Where(r => r.PaymentIdentifier == paymentId).ToListAsync();

            var response = repayments.Select(repayment => new CreateRepaymentResponse
            {
                ID = repayment.ID,
                PaymentIdentifier = repayment.PaymentIdentifier,
                RepaymentAmount = repayment.RepaymentAmount,
                ExternalReference = repayment.ExternalRefrence,
                RepaymentStatus = repayment.RepaymentStatus,
                CreationDate = repayment.CreationDate
            });

            return response;
        }

        private readonly ApplicationDbContext _context;
    }
}
