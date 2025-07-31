using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workshopManagementSystem.Razor.Models;

public class MechanicDTO
{
    public int TechnicianAccountId { get; set; }


    public string TechnicianFullName { get; set; } 
}
