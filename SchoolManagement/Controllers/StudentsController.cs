// File: Controllers/StudentsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;
using SchoolManagement.DTOs;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly SchoolDbContext _context;

    public StudentsController(SchoolDbContext context)
    {
        _context = context;
    }

    // =========================
    // 1. CREATE Student
    // =========================
    [HttpPost]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto dto)
    {
        // 1. Basic Validation (Data Annotations)
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // 2. Custom Validation: Check School Existence
            var schoolExists = await _context.Schools.AnyAsync(s => s.Id == dto.SchoolId);
            if (!schoolExists)
                return BadRequest("The selected School does not exist.");

            // 3. Custom Validation: Check Unique Student ID
            var idExists = await _context.Students.AnyAsync(s => s.StudentCode == dto.StudentCode);
            if (idExists)
                return BadRequest($"Student ID '{dto.StudentCode}' already exists.");

            // 4. Custom Validation: Check Unique Email
            var emailExists = await _context.Students.AnyAsync(s => s.Email == dto.Email);
            if (emailExists)
                return BadRequest($"Email '{dto.Email}' is already taken.");

            // 5. Map DTO to Entity
            var student = new Student
            {
                FullName = dto.FullName,
                StudentCode = dto.StudentCode,
                Email = dto.Email,
                Phone = dto.Phone,
                SchoolId = dto.SchoolId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudents), new { id = student.Id }, "Student created successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // =========================
    // 2. READ – List Students + Pagination
    // =========================
    [HttpGet]
    public async Task<IActionResult> GetStudents([FromQuery] int page = 1)
    {
        try
        {
            int pageSize = 10;
            if (page < 1) page = 1;

            var query = _context.Students.Include(s => s.School).AsQueryable();

            var totalRecords = await query.CountAsync();
            var students = await query
                .OrderByDescending(s => s.CreatedAt) // Good practice to order
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new
                {
                    s.Id,
                    s.FullName,
                    s.StudentCode,
                    s.Email,
                    s.Phone,
                    SchoolName = s.School.Name
                })
                .ToListAsync();

            var response = new
            {
                TotalRecords = totalRecords,
                Page = page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                Data = students
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error retrieving data.");
        }
    }

    // =========================
    // 3. UPDATE Student
    // =========================
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] CreateStudentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return NotFound("Student not found.");

            // Validate School
            if (!await _context.Schools.AnyAsync(s => s.Id == dto.SchoolId))
                return BadRequest("School does not exist.");

            // Validate Unique Email (Exclude current student)
            var emailExists = await _context.Students
                .AnyAsync(s => s.Email == dto.Email && s.Id != id);
            if (emailExists)
                return BadRequest("Email is already taken by another student.");

            // Note: Usually StudentCode is immutable, but if requirement allows update:
            var codeExists = await _context.Students
                .AnyAsync(s => s.StudentCode == dto.StudentCode && s.Id != id);
            if (codeExists)
                return BadRequest("Student ID is already taken by another student.");

            // Update Fields
            student.FullName = dto.FullName;
            student.StudentCode = dto.StudentCode; // If allowed to update
            student.Email = dto.Email;
            student.Phone = dto.Phone;
            student.SchoolId = dto.SchoolId;
            student.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok("Student updated successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating student: {ex.Message}");
        }
    }

    // =========================
    // 4. DELETE Student
    // =========================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        try
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return NotFound("Student not found.");

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return Ok("Student deleted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting student: {ex.Message}");
        }
    }
}