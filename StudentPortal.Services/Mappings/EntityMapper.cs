using StudentPortal.Repositories.Entities;
using StudentPortal.Services.Models;

namespace StudentPortal.Services.Mappings;

public static class EntityMapper
{
    // ── Semester ─────────────────────────────────────────────────────────────

    public static SemesterModel ToModel(this Semester e) => new()
    {
        SemesterId = e.SemesterId,
        SemesterName = e.SemesterName,
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        CourseCount = e.Courses?.Count ?? 0,
        Courses = e.Courses?.Select(c => c.ToModel()).ToList() ?? new()
    };

    public static Semester ToEntity(this SemesterModel m) => new()
    {
        SemesterId   = m.SemesterId,
        SemesterName = m.SemesterName,
        StartDate    = m.StartDate,
        EndDate      = m.EndDate
    };

    // ── Course ────────────────────────────────────────────────────────────────

    public static CourseModel ToModel(this Course e) => new()
    {
        CourseId     = e.CourseId,
        CourseName   = e.CourseName,
        SemesterId   = e.SemesterId,
        SemesterName = e.Semester?.SemesterName ?? string.Empty,
        Subjects     = e.CourseSubjects?.Select(cs => cs.Subject.ToModel()).ToList() ?? new()
    };

    public static Course ToEntity(this CourseModel m) => new()
    {
        CourseId   = m.CourseId,
        CourseName = m.CourseName,
        SemesterId = m.SemesterId
    };

    // ── Subject ───────────────────────────────────────────────────────────────

    public static SubjectModel ToModel(this Subject e) => new()
    {
        SubjectId = e.SubjectId,
        SubjectCode = e.SubjectCode,
        SubjectName = e.SubjectName,
        Credit = e.Credit,
        Courses = e.CourseSubjects?.Select(cs => new SubjectCourseModel
        {
            CourseId = cs.Course?.CourseId ?? cs.CourseId,
            CourseName = cs.Course?.CourseName ?? string.Empty,
            SemesterId = cs.Course?.SemesterId ?? 0,
            SemesterName = cs.Course?.Semester?.SemesterName ?? string.Empty
        }).ToList() ?? new()
    };

    public static Subject ToEntity(this SubjectModel m) => new()
    {
        SubjectId   = m.SubjectId,
        SubjectCode = m.SubjectCode,
        SubjectName = m.SubjectName,
        Credit      = m.Credit
    };

    // ── Student ───────────────────────────────────────────────────────────────

    public static StudentModel ToModel(this Student e) => new()
    {
        StudentId = e.StudentId,
        FullName = e.FullName,
        Email = e.Email,
        Courses = e.Enrollments?.Select(en => new EnrolledCourseModel
        {
            CourseId = en.Course?.CourseId ?? en.CourseId,
            CourseName = en.Course?.CourseName ?? string.Empty,
            SemesterId = en.Course?.SemesterId ?? 0,
            SemesterName = en.Course?.Semester?.SemesterName ?? string.Empty,
            EnrolledAt = en.EnrolledAt
        }).ToList() ?? new()
    };
    public static Student ToEntity(this StudentModel m) => new()
    {
        StudentId = m.StudentId,
        FullName  = m.FullName,
        Email     = m.Email
    };

    // ── Enrollment ────────────────────────────────────────────────────────────

    public static EnrollmentModel ToModel(this Enrollment e) => new()
    {
        EnrollmentId = e.EnrollmentId,
        CourseId     = e.CourseId,
        CourseName   = e.Course?.CourseName ?? string.Empty,
        SemesterName = e.Course?.Semester?.SemesterName ?? string.Empty,
        EnrolledAt   = e.EnrolledAt
    };
}
