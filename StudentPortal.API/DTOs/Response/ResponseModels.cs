namespace StudentPortal.API.DTOs.Response;

// ── Envelope ──────────────────────────────────────────────────────────────────

/// <summary>Standard API response envelope.</summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors };
}

/// <summary>Pagination metadata wrapper.</summary>
public class PagedResponse<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}

// ── Semester ──────────────────────────────────────────────────────────────────

public class SemesterResponse
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CourseCount { get; set; }
}
/// <summary>Expanded semester — populated when $expand=courses is requested.</summary>
public class SemesterExpandedResponse
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CourseCount { get; set; }

    // populated only when $expand=courses
    public List<CourseResponse>? Courses { get; set; }
}
// ── Course ────────────────────────────────────────────────────────────────────

public class CourseResponse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public List<SubjectResponse> Subjects { get; set; } = new();
}

// ── Subject ───────────────────────────────────────────────────────────────────

/// <summary>Base subject response — no navigation properties.</summary>
public class SubjectResponse
{
    public int SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int Credit { get; set; }
}

/// <summary>Expanded subject — populated when $expand=courses is requested.</summary>
public class SubjectExpandedResponse
{
    public int SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int Credit { get; set; }

    // populated only when $expand=courses
    public List<SubjectCourseResponse>? Courses { get; set; }
}

/// <summary>Course info as seen from a subject.</summary>
public class SubjectCourseResponse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
}

// ── Student ───────────────────────────────────────────────────────────────────

/// <summary>Base student response — no navigation properties.</summary>
public class StudentResponse
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>Expanded student — populated when $expand=courses is requested.</summary>
public class StudentExpandedResponse
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // populated only when $expand=enrollments
    public List<EnrollmentResponse>? Enrollments { get; set; }
}

/// <summary>Course info as seen from a student enrollment.</summary>
public class EnrollmentResponse
{
    public int EnrollmentId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
}

/// <summary>
/// Course response with fully expanded navigation objects
/// (used when $expand=semester or $expand=subjects is requested).
/// </summary>
public class CourseExpandedResponse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }

    // populated when $expand=semester
    public SemesterResponse? Semester { get; set; }

    // populated when $expand=subjects
    public List<SubjectResponse>? Subjects { get; set; }
}

public static class SelectProjector
{
    public static object Project(object source, IEnumerable<string> fields)
    {
        var fieldList = fields.ToList();
        if (!fieldList.Any()) return source;

        var props = source.GetType().GetProperties(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);

        var result = new Dictionary<string, object?>();
        foreach (var field in fieldList)
        {
            var prop = props.FirstOrDefault(p =>
                string.Equals(p.Name, field, StringComparison.OrdinalIgnoreCase));
            if (prop is not null)
                result[char.ToLower(prop.Name[0]) + prop.Name[1..]] = prop.GetValue(source);
        }
        return result;
    }

    public static IEnumerable<object> ProjectMany<T>(
        IEnumerable<T> items, IEnumerable<string> fields)
    {
        var fieldList = fields.ToList();
        return !fieldList.Any()
            ? items.Cast<object>()
            : items.Select(item => Project(item!, fieldList));
    }

    /// <summary>Base enrollment response — no navigation properties.</summary>
    /// <summary>Base enrollment — minimal fields only.</summary>
    public class EnrollmentBaseResponse
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }
    }

    /// <summary>Expanded enrollment — student and/or course nested objects.</summary>
    public class EnrollmentExpandedResponse
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }

        // populated when $expand=student
        public StudentResponse? Student { get; set; }

        // populated when $expand=course
        public CourseResponse? Course { get; set; }
    }
}
