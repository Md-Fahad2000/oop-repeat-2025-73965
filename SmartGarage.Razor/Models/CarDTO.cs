using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workshopManagementSystem.Razor.Models
{
    public class CarDTO
    {
        
        public int VehicleRegistrationId { get; set; }

        
        public string VehicleLicenseNumber { get; set; } = null!;

        public int CustomerAccountId { get; set; }

        // Navigation property
        public ICollection<ServiceDTO>? VehicleServiceHistory { get; set; } = new List<ServiceDTO>();
    }
}
