
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using workshopManagementSystem.Razor;
using workshopManagementSystem.Razor.Models;
using workshopManagementSystem.Domain;

namespace workshopManagementSystem.Razor.Controllers
{
    public class AdminController : BaseController
    {
        private readonly MyDbContext context;

        public AdminController(MyDbContext context)
        {
            this.context = context;
        }

        // Check if user is admin
        private bool ValidateAdminAccess()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        [HttpGet]
        public async Task<IActionResult> ShowCustomerList()
        {
            if (!ValidateAdminAccess())
                return RedirectToAction("DisplayAuthenticationForm", "Home");

            var customers = await context.Customers
                .Select(c => new CustomerDTO
                {
                    CustomerAccountId = c.CustomerAccountId,
                    CustomerFullName = c.CustomerFullName,
                    CustomerEmailAddress = c.CustomerEmailAddress,
                    CustomerAccessPassword = c.CustomerAccessPassword,
                }).ToListAsync();

            return View("ShowCustomerList", customers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewCustomer(Customer customer)
        {
            if (!ValidateAdminAccess())
                return RedirectToAction("DisplayAuthenticationForm", "Home");

            bool emailExists = await context.Customers
                .AnyAsync(c => c.CustomerEmailAddress == customer.CustomerEmailAddress);

            if (emailExists)
            {
                ModelState.AddModelError("CustomerEmailAddress", "This email is already registered");
            }

            if (!ModelState.IsValid)
            {
                var allCustomers = await context.Customers
                    .Select(c => new CustomerDTO
                    {
                        CustomerAccountId = c.CustomerAccountId,
                        CustomerFullName = c.CustomerFullName,
                        CustomerEmailAddress = c.CustomerEmailAddress,
                        CustomerAccessPassword = c.CustomerAccessPassword
                    })
                    .ToListAsync();

                ViewBag.NewCustomer = customer;
                return View("ShowCustomerList", allCustomers);
            }

            context.Customers.Add(customer);
            await context.SaveChangesAsync();
            TempData["Success"] = "Customer added successfully!";

            return RedirectToAction("ShowCustomerList");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCustomer(int id)
        {
            if (!ValidateAdminAccess())
                return RedirectToAction("DisplayAuthenticationForm", "Home");

            var customer = context.Customers.Find(id);
            if (customer != null)
            {
                context.Customers.Remove(customer);
                context.SaveChanges();
                TempData["Success"] = "Customer deleted successfully.";
            }

            return RedirectToAction("ShowCustomerList");
        }

        [HttpGet]
        public async Task<IActionResult> ShowCustomerVehicleDetails(int id)
        {
            if (!ValidateAdminAccess())
                return RedirectToAction("DisplayAuthenticationForm", "Home");

            var cars = await context.Cars
                .Include(c => c.VehicleOwner)
                .Include(c => c.VehicleServiceHistory)
                .Where(c => c.CustomerAccountId == id)
                .Select(c => new CarDTO
                {
                    VehicleRegistrationId = c.VehicleRegistrationId,
                    VehicleLicenseNumber = c.VehicleLicenseNumber,
                    CustomerAccountId = c.CustomerAccountId,
                    VehicleServiceHistory = c.VehicleServiceHistory.Select(s => new ServiceDTO
                    {
                        ServiceRecordId = s.ServiceRecordId
                    }).ToList()
                })
                .ToListAsync();

            ViewBag.CustomerId = id;

            var customerName = await context.Customers
                .Where(c => c.CustomerAccountId == id)
                .Select(e => e.CustomerFullName)
                .FirstOrDefaultAsync();

            ViewBag.Name = customerName;

            ViewBag.Mechanics = await context.Mechanics
                .Select(m => new MechanicDTO
                {
                    TechnicianAccountId = m.TechnicianAccountId,
                    TechnicianFullName = m.TechnicianFullName
                })
                .ToListAsync();

            return View("ShowCustomerVehicleDetails", cars);
        }
    }
}

