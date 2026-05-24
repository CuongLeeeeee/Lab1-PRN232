namespace StudentPortal.Repositories.Entities;

/// <summary>Junction table linking Course and Subject (many-to-many)</summary>
public class CourseSubject
{
    public int CourseId { get; set; }
    public int SubjectId { get; set; }

    // Navigation
    public Course Course { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
}
