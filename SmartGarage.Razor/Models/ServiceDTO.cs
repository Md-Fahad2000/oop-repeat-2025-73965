using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using workshopManagementSystem.Domain;

namespace workshopManagementSystem.Razor.Models;

public class ServiceDTO
{

    public int ServiceRecordId { get; set; }

    public int VehicleRegistrationId { get; set; }

    public string VehicleLicenseNumber { get; set; } = string.Empty;

    public int TechnicianAccountId { get; set; }
    public String TechnicianFullName { get; set; }


    [Required]
    public DateTime ServiceStartDate { get; set; }

    public string? ServiceWorkDescription { get; set; }

    [Range(0, int.MaxValue)]
    public decimal ServiceWorkHours { get; set; }

    public DateTime? ServiceCompletionDate { get; set; }

    public decimal? ServiceTotalCost { get; set; }

    public string ServiceCurrentStatus { get; set; } = "Pending";



    public void CalculateServiceCost()
    {
        // Round up hours to next integer
        int hoursRounded = (int)Math.Ceiling(ServiceWorkHours);
        ServiceTotalCost = hoursRounded * 75;
    }
}