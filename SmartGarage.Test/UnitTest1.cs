using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using workshopManagementSystem.Razor;
using workshopManagementSystem.Domain;
using Xunit;

namespace SmartGarage.Test
{
    public class CustomerTests
    {
        [Fact]
        public async Task CreateCustomer_WithValidData_SavesSuccessfully()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase("CustomerCreateDb")
                .Options;

            using var context = new MyDbContext(options);

            var customer = new Customer
            {
                CustomerFullName = "John Doe",
                CustomerEmailAddress = "john.doe@example.com",
                CustomerAccessPassword = "SecurePassword123"
            };

            // Act
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            // Assert
            var savedCustomer = await context.Customers
                .FirstOrDefaultAsync(c => c.CustomerEmailAddress == "john.doe@example.com");

            Assert.NotNull(savedCustomer);
            Assert.Equal("John Doe", savedCustomer.CustomerFullName);
            Assert.Equal("john.doe@example.com", savedCustomer.CustomerEmailAddress);
            Assert.Equal("SecurePassword123", savedCustomer.CustomerAccessPassword);
            Assert.True(savedCustomer.CustomerAccountId > 0); // Verify ID was generated
        }
    }
}