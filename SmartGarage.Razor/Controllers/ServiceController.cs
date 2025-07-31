using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using workshopManagementSystem.Razor;
using workshopManagementSystem.Razor.Models;
using workshopManagementSystem.Domain;

namespace workshopManagementSystem.Razor.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly MyDbContext context;

        public ServiceController(MyDbContext context)
        {
            this.context = context;
        }

        private bool ValidateAdminOrMechanicAccess()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin" || role == "Mechanic";
        }

        [HttpPost]
        public IActionResult ScheduleNewService(Service service)
        {
            if (!ValidateAdminOrMechanicAccess())
                return RedirectToAction("DisplayAuthenticationForm", "Home");

            service.CalculateServiceCost();
            service.ServiceStartDate = DateTime.SpecifyKind(service.ServiceStartDate, DateTimeKind.Utc);

            context.Services.Add(service);
            context.SaveChanges();

            var customerId = context.Cars
                .Where(c => c.VehicleRegistrationId == service.VehicleRegistrationId)
                .Select(c => c.CustomerAccountId)
                .FirstOrDefault();

            return RedirectToAction("ShowCustomerVehicleDetails", "Admin", new { id = customerId });
        }

        [HttpGet]
        public IActionResult RetrieveVehicleServices(int carId)
        {
            try
            {
                var services = context.Services
                    .Where(s => s.VehicleRegistrationId == carId)
                    .Include(s => s.ServiceTechnician)
                    .Select(s => new
                    {
                        ServiceRecordId = s.ServiceRecordId,
                        ServiceStartDate = s.ServiceStartDate,
                        TechnicianFullName = s.ServiceTechnician != null ? s.ServiceTechnician.TechnicianFullName : null,
                        ServiceWorkDescription = s.ServiceWorkDescription,
                        ServiceWorkHours = s.ServiceWorkHours
                    })
                    .ToList();

                return Json(services);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RetrieveVehicleServices: {ex.Message}");
                return StatusCode(500, "Error fetching services");
            }
        }

        [HttpPost]
        public IActionResult RemoveService(int id)
        {
            if (!ValidateAdminOrMechanicAccess())
                return RedirectToAction("DisplayAuthenticationForm", "Home");

            var service = context.Services.FirstOrDefault(s => s.ServiceRecordId == id);
            if (service != null)
            {
                context.Services.Remove(service);
                context.SaveChanges();
                return Ok();
            }
            return NotFound();
        }
    }
}
