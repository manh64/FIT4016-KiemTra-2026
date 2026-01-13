// File: DTOs/CreateStudentDto.cs
namespace SchoolManagement.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateStudentDto
{
    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 100 characters.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Student ID is required.")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "Student ID must be between 5 and 20 characters.")]
    public string StudentCode { get; set; } // Mapping to student_id requirement

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string Email { get; set; }

    [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Phone number must be 10-11 digits.")]
    public string? Phone { get; set; } // Nullable as per requirement

    [Required(ErrorMessage = "School is required.")]
    public int SchoolId { get; set; }
}