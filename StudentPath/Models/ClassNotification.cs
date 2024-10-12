using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StudentPath.Models
{
    public class ClassNotification
    {
        [Key]
        public int ClassNotificationId { get; set; }

        public string Class { get; set; }

        
        public string TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public ApplicationUser Teacher { get; set; }
       

        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student students { get; set; }

        
        public DateOnly Date { get; set; }

     
        public string Status { get; set; } // Status for absence, e.g., "Absent"

    }
}
