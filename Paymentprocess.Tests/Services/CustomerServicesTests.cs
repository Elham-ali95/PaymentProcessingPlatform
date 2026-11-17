
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Paymentprocess.Api.Data;
using Paymentprocess.Api.DTOs;
using Paymentprocess.Api.Services;

namespace Paymentprocess.Tests.Services;

public class CustomerServiceTests
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

    [Fact]
    public async Task CreateCustomerAsync_ValidRequest_CreatesCustomer()
    {
        var (context, connection) = await CreateDatabaseAsync();
        await using (connection)
        await using (context)
        {
            // Arrange
            var service = new CustomerService(context);
            var request = new CreateCustomerRequest
            {
                Firstname = "Test",
                Lastname = "Customer",
                Email = "test.customer@example.com"
            };

            // Act
            var response = await service.CreateCustomerAsync(request);

            // Assert
            Assert.NotEqual(Guid.Empty, response.ID);
            Assert.Equal("Test", response.Firstname);
            Assert.Equal("Customer", response.Lastname);
            Assert.Equal("test.customer@example.com", response.Email);
            Assert.NotEqual(default, response.CreationDate);

            Assert.NotNull(await context.Customers.FindAsync(response.ID));
        }
    }

    [Fact]
    public async Task CreateCustomerAsync_DuplicateEmail_ThrowsDbUpdateException()
    {
        var (context, connection) = await CreateDatabaseAsync();
        await using (connection)
        await using (context)
        {
            // Arrange
            var service = new CustomerService(context);

            var request = new CreateCustomerRequest
            {
                Firstname = "Test",
                Lastname = "Customer",
                Email = "duplicate@example.com"
            };

            await service.CreateCustomerAsync(request);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(
                () => service.CreateCustomerAsync(request));
        }
    }

    [Fact]
    public async Task GetCustomerByIdAsync_NonExistingCustomer_ReturnsNull()
    {
        var (context, connection) = await CreateDatabaseAsync();
        await using (connection)
        await using (context)
        {
            // Arrange
            var service = new CustomerService(context);

            // Act
            var result = await service.GetCustomerByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }
    }
}
