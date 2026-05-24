namespace StudentPortal.Repositories.Entities;

/// <summary>Junction table linking Student and Course (many-to-many enrollment)</summary>
public class Enrollment
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Student Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
