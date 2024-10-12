using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StudentPath.Models
{
    public class ClassAttendence
    {
        [Key]
        public int ClassId { get; set; }

        [Required]
        public string Class { get; set; }

        [Display(Name = "StudentId")]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        [ValidateNever]
        public Student student { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public string Attended { get; set; }

        [Display(Name = "TeacherId")]
        public string TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        [ValidateNever]
        public ApplicationUser Teacher { get; set; }


    }
}
