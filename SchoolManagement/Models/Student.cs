// File: Models/Student.cs (hoặc Student.cs)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// QUAN TRỌNG: Phải có dòng namespace này
namespace SchoolManagement.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SchoolId { get; set; }

        [ForeignKey(nameof(SchoolId))]
        public School? School { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string StudentCode { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [RegularExpression(@"^\d{10,11}$")]
        public string? Phone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}