using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPath.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public string StudentName { get; set; }

        [Display(Name = "StudentId")]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        [ValidateNever]
        public Student student { get; set; }

       
        public string BusNumber { get; set; }
        public DateOnly AttendanceDate { get; set; } // Make sure this property exists
        public string MorningAttendance { get; set; }
        public string AfternoonAttendance { get; set; }






    }

}
