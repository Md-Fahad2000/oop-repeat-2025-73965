using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workshopManagementSystem.Domain;

public class Mechanic
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TechnicianAccountId { get; set; }

    [Required]
    public string TechnicianFullName { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string TechnicianAccessPassword { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string TechnicianEmailAddress { get; set; } = null!;

    public ICollection<Service> TechnicianServiceHistory { get; set; } = new List<Service>();
}
