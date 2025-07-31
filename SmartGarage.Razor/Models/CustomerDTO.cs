using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workshopManagementSystem.Razor.Models;

public class CustomerDTO
{
    public int CustomerAccountId { get; set; }

    
    public string CustomerFullName { get; set; } = null!;

    public string CustomerEmailAddress { get; set; } = null!;

  
    public string CustomerAccessPassword { get; set; } = null!;
}