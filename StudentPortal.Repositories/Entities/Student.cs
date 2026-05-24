namespace StudentPortal.Repositories.Entities;

public class Student
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Navigation
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
