using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Entities;
using StudentPortal.Repositories.Interfaces;
using StudentPortal.Services.Interfaces;
using StudentPortal.Services.Mappings;
using StudentPortal.Services.Models;

namespace StudentPortal.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repo;

    public EnrollmentService(IEnrollmentRepository repo) => _repo = repo;

    public async Task<PagedResult<EnrollmentListModel>> GetAllAsync(
    QueryParameters parameters,
    int? studentId = null,
    int? courseId = null,
    bool includeStudent = false,
    bool includeCourse = false)
    {
        var paged = await _repo.SearchAsync(
            parameters, studentId, courseId, includeStudent, includeCourse);

        return new PagedResult<EnrollmentListModel>
        {
            Items = paged.Items.Select(e => e.ToListModel()),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
    }

    public async Task<EnrollmentListModel?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity?.ToListModel();
    }

    public async Task<EnrollmentListModel> CreateAsync(int studentId, int courseId)
    {
        if (!await _repo.StudentExistsAsync(studentId))
            throw new InvalidOperationException($"Student with ID {studentId} does not exist.");

        if (!await _repo.CourseExistsAsync(courseId))
            throw new InvalidOperationException($"Course with ID {courseId} does not exist.");

        if (await _repo.ExistsAsync(studentId, courseId))
            throw new InvalidOperationException(
                $"Student {studentId} is already enrolled in Course {courseId}.");

        var entity = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow
        };

        var created = await _repo.CreateAsync(entity);

        // reload with navigation properties
        var full = await _repo.GetByIdAsync(created.EnrollmentId);
        return full!.ToListModel();
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);
}