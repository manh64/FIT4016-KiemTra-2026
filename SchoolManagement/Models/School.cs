// File: Models/School.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// BẮT BUỘC PHẢI CÓ NAMESPACE NÀY
namespace SchoolManagement.Models
{
    public class School
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Principal { get; set; }

        [Required]
        public string Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        // Khi School và Student cùng nằm trong namespace SchoolManagement.Models
        // thì dòng này sẽ không bị lỗi nữa.
        public ICollection<Student> Students { get; set; }
    }
}