using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StudentPath.Models
{
    public class BusAttendence
    {
        [Key]
        public int BusId { get; set; }
        [Required]
        public string BusNumber { get; set; }

        [Display(Name = "StudentId")]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        [ValidateNever]
        public Student student { get; set; }


        [Required]
        public DateOnly Date { get; set; }

        public string Morning_Attendance { get; set; }
        public string Afternoon_Attendance { get; set; }

        [Display(Name = "Bus_DriverId")]
        public string Bus_DriverId { get; set; }
        [ForeignKey("Bus_DriverId")]
        [ValidateNever]
        public ApplicationUser Bus_Driver { get; set; }

    }
}
