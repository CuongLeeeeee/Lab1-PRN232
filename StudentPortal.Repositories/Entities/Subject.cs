namespace StudentPortal.Repositories.Entities;

public class Subject
{
    public int SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int Credit { get; set; }

    // Navigation
    public ICollection<CourseSubject> CourseSubjects { get; set; } = new List<CourseSubject>();
}
