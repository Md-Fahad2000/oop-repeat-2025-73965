using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workshopManagementSystem.Domain;

public class Customer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CustomerAccountId { get; set; }

    [Required]
    public string CustomerFullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string CustomerEmailAddress { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string CustomerAccessPassword { get; set; } = null!;
    public List<int>? CustomerVehicleIds = new List<int>();
    public ICollection<Car>? CustomerVehicles { get; set; } = new List<Car>();
}