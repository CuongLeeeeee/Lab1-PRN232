using StudentPortal.API.DTOs.Response;
using StudentPortal.Repositories.Common;
using StudentPortal.Services.Models;

namespace StudentPortal.API.Extensions;

public static class ResponseMapper
{
    // ── Semester ──────────────────────────────────────────────────────────────

    public static SemesterResponse ToResponse(this SemesterModel m) => new()
    {
        SemesterId   = m.SemesterId,
        SemesterName = m.SemesterName,
        StartDate    = m.StartDate,
        EndDate      = m.EndDate,
        CourseCount  = m.CourseCount
    };

    public static SemesterModel ToModel(this DTOs.Request.CreateSemesterRequest r) => new()
    {
        SemesterName = r.SemesterName,
        StartDate    = r.StartDate,
        EndDate      = r.EndDate
    };

    // ── Course ────────────────────────────────────────────────────────────────

    public static CourseResponse ToResponse(this CourseModel m) => new()
    {
        CourseId     = m.CourseId,
        CourseName   = m.CourseName,
        SemesterId   = m.SemesterId,
        Subjects     = m.Subjects.Select(s => s.ToResponse()).ToList()
    };

    public static CourseModel ToModel(this DTOs.Request.CreateCourseRequest r) => new()
    {
        CourseName = r.CourseName,
        SemesterId = r.SemesterId
    };

    // ── Subject ───────────────────────────────────────────────────────────────

    public static SubjectResponse ToResponse(this SubjectModel m) => new()
    {
        SubjectId = m.SubjectId,
        SubjectCode = m.SubjectCode,
        SubjectName = m.SubjectName,
        Credit = m.Credit
    };

    public static SubjectExpandedResponse ToExpandedResponse(
        this SubjectModel m,
        IEnumerable<string> expands)
    {
        var expandList = expands.ToList();
        return new SubjectExpandedResponse
        {
            SubjectId = m.SubjectId,
            SubjectCode = m.SubjectCode,
            SubjectName = m.SubjectName,
            Credit = m.Credit,

            Courses = expandList.Contains("courses")
                ? m.Courses.Select(c => new SubjectCourseResponse
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    SemesterId = c.SemesterId,
                    SemesterName = c.SemesterName
                }).ToList()
                : null
        };
    }

    public static SubjectModel ToModel(this DTOs.Request.CreateSubjectRequest r) => new()
    {
        SubjectCode = r.SubjectCode,
        SubjectName = r.SubjectName,
        Credit      = r.Credit
    };

    // ── Student ───────────────────────────────────────────────────────────────

    public static StudentResponse ToResponse(this StudentModel m) => new()
    {
        StudentId = m.StudentId,
        FullName = m.FullName,
        Email = m.Email
    };

    public static StudentExpandedResponse ToExpandedResponse(
        this StudentModel m,
        IEnumerable<string> expands)
    {
        var expandList = expands.ToList();
        return new StudentExpandedResponse
        {
            StudentId = m.StudentId,
            FullName = m.FullName,
            Email = m.Email,

            Courses = expandList.Contains("courses")
                ? m.Courses.Select(c => new EnrolledCourseResponse
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    SemesterId = c.SemesterId,
                    SemesterName = c.SemesterName,
                    EnrolledAt = c.EnrolledAt
                }).ToList()
                : null
        };
    }

    public static StudentModel ToModel(this DTOs.Request.CreateStudentRequest r) => new()
    {
        FullName = r.FullName,
        Email    = r.Email
    };

    // ── Enrollment ────────────────────────────────────────────────────────────

    public static EnrollmentResponse ToResponse(this EnrollmentModel m) => new()
    {
        EnrollmentId = m.EnrollmentId,
        CourseId     = m.CourseId,
        CourseName   = m.CourseName,
        SemesterName = m.SemesterName,
        EnrolledAt   = m.EnrolledAt
    };

    // ── PagedResult helper ────────────────────────────────────────────────────

    public static PagedResponse<TResponse> ToPagedResponse<TModel, TResponse>(
        this PagedResult<TModel> paged,
        Func<TModel, TResponse> map) => new()
    {
        Items           = paged.Items.Select(map),
        TotalCount      = paged.TotalCount,
        Page            = paged.Page,
        PageSize        = paged.PageSize,
        TotalPages      = paged.TotalPages,
        HasPreviousPage = paged.HasPreviousPage,
        HasNextPage     = paged.HasNextPage
    };

    public static CourseExpandedResponse ToExpandedResponse(
    this CourseModel m,
    IEnumerable<string> expands)
    {
        var expandList = expands.ToList();
        return new CourseExpandedResponse
        {
            CourseId = m.CourseId,
            CourseName = m.CourseName,
            SemesterId = m.SemesterId,

            Semester = expandList.Contains("semester")
                ? new SemesterResponse
                {
                    SemesterId = m.SemesterId,
                    SemesterName = m.SemesterName
                }
                : null,

            Subjects = expandList.Contains("subjects")
                ? m.Subjects.Select(s => s.ToResponse()).ToList()
                : null
        };
    }
    public static SemesterExpandedResponse ToExpandedResponse(
    this SemesterModel m,
    IEnumerable<string> expands)
    {
        var expandList = expands.ToList();
        return new SemesterExpandedResponse
        {
            SemesterId = m.SemesterId,
            SemesterName = m.SemesterName,
            StartDate = m.StartDate,
            EndDate = m.EndDate,
            CourseCount = m.CourseCount,

            Courses = expandList.Contains("courses")
                ? m.Courses.Select(c => c.ToResponse()).ToList()
                : null
        };
    }

}
