using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workshopManagementSystem.Domain
{
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdministratorAccountId { get; set; }

        [Required]
        public string AdministratorFullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string AdministratorEmailAddress { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string AdministratorAccessPassword { get; set; } = null!;
       
    }
}
