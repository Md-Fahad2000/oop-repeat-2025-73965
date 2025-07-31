using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workshopManagementSystem.Domain;

public class Car
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VehicleRegistrationId { get; set; }

    [Required]
    public string VehicleLicenseNumber { get; set; } = null!;

    public int CustomerAccountId { get; set; }
    public Customer? VehicleOwner { get; set; } = null!;

    public ICollection<Service>? VehicleServiceHistory { get; set; } = new List<Service>();
}
