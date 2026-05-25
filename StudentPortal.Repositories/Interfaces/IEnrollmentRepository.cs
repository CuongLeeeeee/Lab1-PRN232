using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Entities;

namespace StudentPortal.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(int id);
    Task<PagedResult<Enrollment>> SearchAsync(QueryParameters parameters,
    int? studentId = null, int? courseId = null,
    bool includeStudent = false, bool includeCourse = false);
    Task<Enrollment> CreateAsync(Enrollment entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int studentId, int courseId);
    Task<bool> StudentExistsAsync(int studentId);
    Task<bool> CourseExistsAsync(int courseId);
}