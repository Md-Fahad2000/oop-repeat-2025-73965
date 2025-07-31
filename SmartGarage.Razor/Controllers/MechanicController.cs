using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using workshopManagementSystem.Razor;
using workshopManagementSystem.Razor.Models;
using workshopManagementSystem.Domain;

namespace workshopManagementSystem.Razor.Controllers
{
    public class MechanicController : BaseController
    {
        private readonly MyDbContext context;

        public MechanicController(MyDbContext context)
        {
            this.context = context;
        }

        private bool ValidateMechanicAccess()
        {
            return HttpContext.Session.GetString("UserRole") == "Mechanic";
        }

        private int GetCurrentMechanicId()
        {
            string sessionId = HttpContext.Session.GetString("UserSession");
            int.TryParse(sessionId, out int mechanicId);
            return mechanicId;
        }

        public async Task<IActionResult> ShowMechanicDashboard()
        {
            if (!ValidateMechanicAccess())
                return RedirectToAction("DisplayAuthenticationForm", "Home");

            int mechanicId = GetCurrentMechanicId();
            if (mechanicId == 0)
                return RedirectToAction("DisplayAuthenticationForm", "Home");

            var services = await context.Services
                .Include(s => s.ServiceVehicle)
                .Include(s => s.ServiceTechnician)
                .Where(s => s.TechnicianAccountId == mechanicId)
                .Select(s => new ServiceDTO
                {
                    ServiceRecordId = s.ServiceRecordId,
                    ServiceStartDate = s.ServiceStartDate,
                    TechnicianFullName = s.ServiceTechnician != null ? s.ServiceTechnician.TechnicianFullName : string.Empty,
                    ServiceWorkDescription = s.ServiceWorkDescription,
                    ServiceWorkHours = s.ServiceWorkHours,
                    VehicleRegistrationId = s.VehicleRegistrationId,
                    TechnicianAccountId = s.TechnicianAccountId,
                    ServiceTotalCost = s.ServiceTotalCost,
                    VehicleLicenseNumber = s.ServiceVehicle != null ? s.ServiceVehicle.VehicleLicenseNumber : string.Empty,
                    ServiceCurrentStatus = s.ServiceCurrentStatus ?? "Pending",
                    ServiceCompletionDate = s.ServiceCompletionDate
                })
                .ToListAsync();

            return View("ShowMechanicDashboard", services);
        }

        [HttpPost]
        public async Task<IActionResult> FinalizeServiceCompletion(int id, string repairDescription, decimal hours)
        {
            if (!ValidateMechanicAccess())
                return RedirectToAction("DisplayAuthenticationForm", "Home");

            var service = context.Services
                .Include(s => s.ServiceVehicle)
                .FirstOrDefault(s => s.ServiceRecordId == id);

            if (service == null)
            {
                return NotFound();
            }

            service.ServiceWorkDescription = repairDescription;
            service.ServiceWorkHours = hours;
            service.ServiceCurrentStatus = "Completed";
            service.ServiceCompletionDate = DateTime.UtcNow;
            service.ServiceTotalCost = hours * 75;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error: {ex.InnerException?.Message}");
                throw;
            }

            return RedirectToAction("ShowMechanicDashboard");
        }
    }
}