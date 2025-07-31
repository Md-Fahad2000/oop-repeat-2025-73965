using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workshopManagementSystem.Domain;

public class Service
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ServiceRecordId { get; set; }

    public int VehicleRegistrationId { get; set; }
    public Car ServiceVehicle { get; set; } = null!;

    public int TechnicianAccountId { get; set; }
    public Mechanic ServiceTechnician { get; set; } = null!;
   


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
        int hoursRounded = (int)Math.Ceiling(ServiceWorkHours);
        ServiceTotalCost = hoursRounded * 75;
    }
}