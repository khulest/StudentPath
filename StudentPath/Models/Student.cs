using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudentPath.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        [Required]
        public string StudentName { get; set; }
        [Required]
        public string StudentSurname { get; set; }
        [Required]
        public int StudentAge { get; set; }
        public string StudentGender { get; set; }
        [Required]
        public string StudentGrade { get; set; }
        public string StudentAddress { get; set; }

        [Display(Name = "parentId")]
        public string parentId { get; set; }
        [ForeignKey("parentId")]
        [ValidateNever]
        public ApplicationUser parent { get; set; }

    }
}
