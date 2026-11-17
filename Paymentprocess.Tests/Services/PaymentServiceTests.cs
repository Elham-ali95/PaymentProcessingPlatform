
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Paymentprocess.Api.Data;
using Paymentprocess.Api.DTOs;
using Paymentprocess.Api.Enums;
using Paymentprocess.Api.Models;
using Paymentprocess.Api.Services;

namespace Paymentprocess.Tests.Services;

public class PaymentServiceTests
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

    private static async Task<Guid> AddCustomerAsync(ApplicationDbContext context)
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
        await context.SaveChangesAsync();

        return customer.ID;
    }

    [Fact]
    public async Task CreatePaymentAsync_ValidRequest_CreatesPaymentAndTransaction()
    {
        var (context, connection) = await CreateDatabaseAsync();
        await using (connection)
        await using (context)
        {
            // Arrange
            var customerId = await AddCustomerAsync(context);
            var service = new PaymentService(context);

            var request = new CreatePaymentRequest
            {
                CustomerID = customerId,
                MerchantReference = "ORDER-001",
                OriginalPaymentAmount = 100m,
                Currency = "SEK"
            };

            // Act
            var response = await service.CreatePaymentAsync(request);

            // Assert
            Assert.NotEqual(Guid.Empty, response.ID);
            Assert.Equal(customerId, response.CustomerID);
            Assert.Equal(100m, response.OriginalPaymentAmount);
            Assert.Equal(100m, response.OutstandingAmount);
            Assert.Equal("SEK", response.Currency);
            Assert.Equal(PaymentStatus.Pending, response.PaymentStatus);

            var transaction = await context.Transactions
                .SingleAsync(t => t.PaymentID == response.ID);

            Assert.Equal(TransactionType.PaymentCreated, transaction.TransactionType);
            Assert.Equal(100m, transaction.Amount);
            Assert.Equal("ORDER-001", transaction.Reference);
        }
    }

    [Fact]
    public async Task CreatePaymentAsync_NonExistingCustomer_ThrowsArgumentException()
    {
        var (context, connection) = await CreateDatabaseAsync();
        await using (connection)
        await using (context)
        {
            // Arrange
            var service = new PaymentService(context);

            var request = new CreatePaymentRequest
            {
                CustomerID = Guid.NewGuid(),
                MerchantReference = "ORDER-002",
                OriginalPaymentAmount = 100m,
                Currency = "SEK"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => service.CreatePaymentAsync(request));

            Assert.Empty(await context.Payments.ToListAsync());
            Assert.Empty(await context.Transactions.ToListAsync());
        }
    }

    [Fact]
    public async Task CreatePaymentAsync_ZeroAmount_ThrowsArgumentException()
    {
        var (context, connection) = await CreateDatabaseAsync();
        await using (connection)
        await using (context)
        {
            // Arrange
            var customerId = await AddCustomerAsync(context);
            var service = new PaymentService(context);

            var request = new CreatePaymentRequest
            {
                CustomerID = customerId,
                MerchantReference = "ORDER-003",
                OriginalPaymentAmount = 0m,
                Currency = "SEK"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => service.CreatePaymentAsync(request));

            Assert.Empty(await context.Payments.ToListAsync());
            Assert.Empty(await context.Transactions.ToListAsync());
        }
    }
}
