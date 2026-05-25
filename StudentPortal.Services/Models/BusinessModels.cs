namespace StudentPortal.Services.Models;

// ─── Semester ────────────────────────────────────────────────────────────────

public class SemesterModel
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CourseCount { get; set; }

    // populated when $expand=courses
    public List<CourseModel> Courses { get; set; } = new();
}

// ─── Course ───────────────────────────────────────────────────────────────────

public class CourseModel
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public List<SubjectModel> Subjects { get; set; } = new();
}

// ─── Subject ──────────────────────────────────────────────────────────────────

public class SubjectModel
{
    public int SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int Credit { get; set; }

    // populated when $expand=courses
    public List<SubjectCourseModel> Courses { get; set; } = new();
}

public class SubjectCourseModel
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
}

// ─── Student ──────────────────────────────────────────────────────────────────

public class StudentModel
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // populated when $expand=enrollments
    public List<EnrollmentModel> Enrollments { get; set; } = new();
}

public class EnrollmentModel
{
    public int EnrollmentId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
}

public class EnrolledCourseModel
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
}

public class EnrollmentListModel
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrolledAt { get; set; }

    // populated when $expand=student
    public StudentModel? Student { get; set; }

    // populated when $expand=course
    public CourseModel? Course { get; set; }
}