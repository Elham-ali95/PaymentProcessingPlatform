
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Paymentprocess.Api.Data;
using Paymentprocess.Api.DTOs;
using Paymentprocess.Api.Enums;
using Paymentprocess.Api.Models;
using Paymentprocess.Api.Services;

namespace Paymentprocess.Tests.Services;

public class RepaymentServiceTests
{
    private static async Task<(ApplicationDbContext Context, SqliteConnection Connection)> CreateDatabaseAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        return (context, connection);
    }

    private static async Task<Guid> AddPaymentAsync(
        ApplicationDbContext context,
        PaymentStatus status = PaymentStatus.Pending)
    {
        var customer = new Customers
        {
            ID = Guid.NewGuid(),
            Firstname = "Test",
            Lastname = "Customer",
            Email = $"{Guid.NewGuid()}@example.com",
            CreationDate = DateTime.UtcNow
        };

        context.Customers.Add(customer);

        var payment = new Payments
        {
            ID = Guid.NewGuid(),
            CustomerID = customer.ID,
            MerchantReference = "ORDER-REP",
            OriginalPaymentAmount = 100m,
            OutstandingAmount = 100m,
            Currency = "SEK",
            PaymentStatus = status,
            CreationDate = DateTime.UtcNow
        };

        context.Payments.Add(payment);
        await context.SaveChangesAsync();

        return payment.ID;
    }

    [Fact]
    public async Task CreateRepaymentAsync_FullRepayment_UpdatesPaymentAndCreatesRecords()
    {
        var (context, connection) = await CreateDatabaseAsync();
        await using (connection)
        await using (context)
        {
            // Arrange
            var paymentId = await AddPaymentAsync(context);
            var service = new RepaymentService(context);

            var request = new CreateRepaymentRequest
            {
                RepaymentAmount = 100m,
                ExternalReference = "REF-001"
            };

            // Act
            var response = await service.CreateRepaymentAsync(
                paymentId,
                request,
                "idempotency-001");

            // Assert
            Assert.Equal(paymentId, response.PaymentIdentifier);
            Assert.Equal(100m, response.RepaymentAmount);
            Assert.Equal(RepaymentStatus.Completed, response.RepaymentStatus);

            var payment = await context.Payments.FindAsync(paymentId);

            Assert.NotNull(payment);
            Assert.Equal(0m, payment.OutstandingAmount);
            Assert.Equal(PaymentStatus.Paid, payment.PaymentStatus);

            Assert.Equal(1, await context.Repayments.CountAsync());

            Assert.Equal(
                1,
                await context.Transactions.CountAsync(
                    t => t.TransactionType == TransactionType.RepaymentReceived));

            Assert.Equal(1, await context.IdempotencyRecords.CountAsync());
        }
    }

    [Fact]
    public async Task CreateRepaymentAsync_AmountExceedsOutstanding_ThrowsAndDoesNotChangePayment()
    {
        var (context, connection) = await CreateDatabaseAsync();
        await using (connection)
        await using (context)
        {
            // Arrange
            var paymentId = await AddPaymentAsync(context);
            var service = new RepaymentService(context);

            var request = new CreateRepaymentRequest
            {
                RepaymentAmount = 150m,
                ExternalReference = "REF-002"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => service.CreateRepaymentAsync(
                    paymentId,
                    request,
                    "idempotency-002"));

            var payment = await context.Payments.FindAsync(paymentId);

            Assert.NotNull(payment);
            Assert.Equal(100m, payment.OutstandingAmount);
            Assert.Equal(PaymentStatus.Pending, payment.PaymentStatus);

            Assert.Empty(await context.Repayments.ToListAsync());
            Assert.Empty(await context.Transactions.ToListAsync());
        }
    }

    [Fact]
    public async Task CreateRepaymentAsync_CancelledPayment_ThrowsAndDoesNotCreateRepayment()
    {
        var (context, connection) = await CreateDatabaseAsync();
        await using (connection)
        await using (context)
        {
            // Arrange
            var paymentId = await AddPaymentAsync(
                context,
                PaymentStatus.Cancelled);

            var service = new RepaymentService(context);

            var request = new CreateRepaymentRequest
            {
                RepaymentAmount = 25m,
                ExternalReference = "REF-003"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CreateRepaymentAsync(
                    paymentId,
                    request,
                    "idempotency-003"));

            Assert.Empty(await context.Repayments.ToListAsync());
            Assert.Empty(await context.Transactions.ToListAsync());
        }
    }
}
