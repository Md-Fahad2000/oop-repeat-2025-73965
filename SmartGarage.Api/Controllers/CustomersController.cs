using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using workshopManagementSystem.Razor;
using workshopManagementSystem.Razor.Models;
using workshopManagementSystem.Domain;

namespace SmartGarage.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly MyDbContext _context;

        public CustomersController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetCustomers()
        {
            try
            {
                var customers = await _context.Customers
                    .Select(c => new CustomerDTO
                    {
                        CustomerAccountId = c.CustomerAccountId,
                        CustomerFullName = c.CustomerFullName,
                        CustomerEmailAddress = c.CustomerEmailAddress,
                        CustomerAccessPassword = c.CustomerAccessPassword
                    })
                    .ToListAsync();

                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomer(int id)
        {
            try
            {
                var customer = await _context.Customers
                    .Where(c => c.CustomerAccountId == id)
                    .Select(c => new CustomerDTO
                    {
                        CustomerAccountId = c.CustomerAccountId,
                        CustomerFullName = c.CustomerFullName,
                        CustomerEmailAddress = c.CustomerEmailAddress,
                        CustomerAccessPassword = c.CustomerAccessPassword
                    })
                    .FirstOrDefaultAsync();

                if (customer == null)
                {
                    return NotFound($"Customer with ID {id} not found");
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 