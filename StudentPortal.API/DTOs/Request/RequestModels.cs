using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.API.DTOs.Request;

// ── Semester ──────────────────────────────────────────────────────────────────

/// <summary>Payload for creating a new Semester.</summary>
public class CreateSemesterRequest
{
    /// <example>Spring 2025</example>
    [Required, MaxLength(100)]
    public string SemesterName { get; set; } = string.Empty;

    /// <example>2025-01-06</example>
    [Required]
    public DateTime StartDate { get; set; }

    /// <example>2025-05-23</example>
    [Required]
    public DateTime EndDate { get; set; }
}

/// <summary>Payload for updating an existing Semester.</summary>
public class UpdateSemesterRequest : CreateSemesterRequest { }

// ── Course ────────────────────────────────────────────────────────────────────

/// <summary>Payload for creating a new Course.</summary>
public class CreateCourseRequest
{
    /// <example>PRN232 – ASP.NET Web API Development</example>
    [Required, MaxLength(200)]
    public string CourseName { get; set; } = string.Empty;

    /// <example>1</example>
    [Required, Range(1, int.MaxValue, ErrorMessage = "SemesterId must be a positive integer.")]
    public int SemesterId { get; set; }
}

/// <summary>Payload for updating an existing Course.</summary>
public class UpdateCourseRequest : CreateCourseRequest { }

// ── Subject ───────────────────────────────────────────────────────────────────

/// <summary>Payload for creating a new Subject.</summary>
public class CreateSubjectRequest
{
    /// <example>PRN232</example>
    [Required, MaxLength(20)]
    public string SubjectCode { get; set; } = string.Empty;

    /// <example>ASP.NET Web API</example>
    [Required, MaxLength(200)]
    public string SubjectName { get; set; } = string.Empty;

    /// <example>3</example>
    [Required, Range(1, 10, ErrorMessage = "Credit must be between 1 and 10.")]
    public int Credit { get; set; }
}

/// <summary>Payload for updating an existing Subject.</summary>
public class UpdateSubjectRequest : CreateSubjectRequest { }

// ── Student ───────────────────────────────────────────────────────────────────

/// <summary>Payload for creating a new Student.</summary>
public class CreateStudentRequest
{
    /// <example>Nguyen Van An</example>
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    /// <example>an.nguyen@fpt.edu.vn</example>
    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;
}

/// <summary>Payload for updating an existing Student.</summary>
public class UpdateStudentRequest : CreateStudentRequest { }

// ── Shared query parameters ───────────────────────────────────────────────────

public class PaginationRequest
{
    /// <example>1</example>
    public int Page { get; set; } = 1;

    /// <example>10</example>
    public int PageSize { get; set; } = 10;

    public string? Search { get; set; }

    public string? SortBy { get; set; }

    /// <example>false</example>
    public bool SortDescending { get; set; } = false;

    [FromQuery(Name = "$select")]
    public string? Select { get; set; }

    [FromQuery(Name = "$expand")]
    public string? Expand { get; set; }

}
