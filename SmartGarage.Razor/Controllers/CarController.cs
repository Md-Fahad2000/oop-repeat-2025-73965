using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using workshopManagementSystem.Razor;
using workshopManagementSystem.Domain;

namespace workshopManagementSystem.Razor.Controllers
{
    public class CarController : BaseController
    {
        private readonly MyDbContext context;

        public CarController(MyDbContext context)
        {
            this.context = context;
        }

        private bool ValidateAdminAccess()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        [HttpPost]
        public IActionResult RegisterNewVehicle(string VehicleLicenseNumber, int CustomerAccountId)
        {
            if (!ValidateAdminAccess())
                return RedirectToAction("DisplayAuthenticationForm", "Home");

            var exists = context.Cars.Any(c => c.VehicleLicenseNumber == VehicleLicenseNumber && c.CustomerAccountId == CustomerAccountId);

            if (exists)
            {
                TempData["CarExists"] = "Car with this license plate already exists.";
                return RedirectToAction("ShowCustomerVehicleDetails", "Admin", new { id = CustomerAccountId });
            }

            var newCar = new Car
            {
                VehicleLicenseNumber = VehicleLicenseNumber,
                CustomerAccountId = CustomerAccountId
            };

            context.Cars.Add(newCar);
            context.SaveChanges();

            return RedirectToAction("ShowCustomerVehicleDetails", "Admin", new { id = CustomerAccountId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveVehicle(int id)
        {
            if (!ValidateAdminAccess())
                return RedirectToAction("DisplayAuthenticationForm", "Home");

            var car = context.Cars.Include(c => c.VehicleServiceHistory)
                                   .FirstOrDefault(c => c.VehicleRegistrationId == id);

            if (car == null)
            {
                return NotFound();
            }

            if (car.VehicleServiceHistory.Any())
            {
                context.Services.RemoveRange(car.VehicleServiceHistory);
            }

            context.Cars.Remove(car);
            context.SaveChanges();

            return Ok();
        }
    }
}