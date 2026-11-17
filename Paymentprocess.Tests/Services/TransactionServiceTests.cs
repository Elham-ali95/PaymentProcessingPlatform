
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Paymentprocess.Api.Data;
using Paymentprocess.Api.Enums;
using Paymentprocess.Api.Models;
using Paymentprocess.Api.Services;

namespace Paymentprocess.Tests.Services;

public class TransactionServiceTests
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

    private static async Task<Guid> AddPaymentAsync(ApplicationDbContext context)
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
            MerchantReference = "ORDER-TX",
            OriginalPaymentAmount = 100m,
            OutstandingAmount = 100m,
            Currency = "SEK",
            PaymentStatus = PaymentStatus.Pending,
            CreationDate = DateTime.UtcNow
        };

        context.Payments.Add(payment);
        await context.SaveChangesAsync();

        return payment.ID;
    }

    [Fact]
    public async Task GetTransactionsByPaymentIdAsync_ExistingPayment_ReturnsTransactions()
    {
        var (context, connection) = await CreateDatabaseAsync();
        await using (connection)
        await using (context)
        {
            // Arrange
            var paymentId = await AddPaymentAsync(context);

            context.Transactions.Add(new Transactions
            {
                ID = Guid.NewGuid(),
                PaymentID = paymentId,
                TransactionType = TransactionType.PaymentCreated,
                Amount = 100m,
                Reference = "ORDER-TX",
                CreationDate = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var service = new TransactionServices(context);

            // Act
            var result = (await service.GetTransactionsByPaymentIdAsync(paymentId))
                .ToList();

            // Assert
            var transaction = Assert.Single(result);

            Assert.Equal(TransactionType.PaymentCreated, transaction.TransactionType);
            Assert.Equal(100m, transaction.Amount);
        }
    }

    [Fact]
    public async Task GetTransactionsByPaymentIdAsync_MultipleTransactions_ReturnsChronologicalOrder()
    {
        var (context, connection) = await CreateDatabaseAsync();
        await using (connection)
        await using (context)
        {
            // Arrange
            var paymentId = await AddPaymentAsync(context);

            var earlier = DateTime.UtcNow.AddMinutes(-5);
            var later = DateTime.UtcNow;

            context.Transactions.AddRange(
                new Transactions
                {
                    ID = Guid.NewGuid(),
                    PaymentID = paymentId,
                    TransactionType = TransactionType.RepaymentReceived,
                    Amount = 25m,
                    Reference = "REF-2",
                    CreationDate = later
                },
                new Transactions
                {
                    ID = Guid.NewGuid(),
                    PaymentID = paymentId,
                    TransactionType = TransactionType.PaymentCreated,
                    Amount = 100m,
                    Reference = "ORDER-TX",
                    CreationDate = earlier
                });

            await context.SaveChangesAsync();

            var service = new TransactionServices(context);

            // Act
            var result = (await service.GetTransactionsByPaymentIdAsync(paymentId))
                .ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(TransactionType.PaymentCreated, result[0].TransactionType);
            Assert.Equal(TransactionType.RepaymentReceived, result[1].TransactionType);
        }
    }

    [Fact]
    public async Task GetTransactionsByPaymentIdAsync_NonExistingPayment_ThrowsArgumentException()
    {
        var (context, connection) = await CreateDatabaseAsync();
        await using (connection)
        await using (context)
        {
            // Arrange
            var service = new TransactionServices(context);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => service.GetTransactionsByPaymentIdAsync(Guid.NewGuid()));
        }
    }
}
