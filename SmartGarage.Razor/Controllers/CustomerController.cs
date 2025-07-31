using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using workshopManagementSystem.Razor;
using workshopManagementSystem.Razor.Models;
using workshopManagementSystem.Domain;

namespace workshopManagementSystem.Razor.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly MyDbContext context;

        public CustomerController(MyDbContext context)
        {
            this.context = context;
        }

        private bool ValidateCustomerAccess()
        {
            return HttpContext.Session.GetString("UserRole") == "Customer";
        }

        private int GetCurrentCustomerId()
        {
            string sessionId = HttpContext.Session.GetString("UserSession");
            int.TryParse(sessionId, out int customerId);
            return customerId;
        }

        public async Task<IActionResult> ShowCustomerDashboard()
        {
            if (!ValidateCustomerAccess())
                return RedirectToAction("DisplayAuthenticationForm", "Home");

            int customerId = GetCurrentCustomerId();
            if (customerId == 0)
                return RedirectToAction("DisplayAuthenticationForm", "Home");

            var customerCars = await context.Cars
                .Where(car => car.CustomerAccountId == customerId)
                .Include(car => car.VehicleServiceHistory)
                    .ThenInclude(service => service.ServiceTechnician)
                .ToListAsync();

            var carDTOs = customerCars.Select(car => new CarDTO
            {
                VehicleRegistrationId = car.VehicleRegistrationId,
                VehicleLicenseNumber = car.VehicleLicenseNumber,
                VehicleServiceHistory = car.VehicleServiceHistory.Select(s => new ServiceDTO
                {
                    ServiceRecordId = s.ServiceRecordId,
                    ServiceStartDate = s.ServiceStartDate,
                    TechnicianFullName = s.ServiceTechnician != null ? s.ServiceTechnician.TechnicianFullName : "N/A",
                    ServiceWorkDescription = s.ServiceWorkDescription,
                    ServiceCurrentStatus = s.ServiceCurrentStatus,
                    ServiceWorkHours = s.ServiceWorkHours,
                    ServiceCompletionDate = s.ServiceCompletionDate,
                    ServiceTotalCost = s.ServiceTotalCost
                }).ToList()
            }).ToList();

            return View("ShowCustomerDashboard", carDTOs);
        }
    }
}