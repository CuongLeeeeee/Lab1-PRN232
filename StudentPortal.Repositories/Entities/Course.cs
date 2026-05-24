namespace StudentPortal.Repositories.Entities;

public class Course
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }

    // Navigation
    public Semester Semester { get; set; } = null!;
    public ICollection<CourseSubject> CourseSubjects { get; set; } = new List<CourseSubject>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
