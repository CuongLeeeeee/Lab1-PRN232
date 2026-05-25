using StudentPortal.Repositories.Common;
using StudentPortal.Services.Models;

namespace StudentPortal.Services.Interfaces;

public interface IEnrollmentService
{
    Task<PagedResult<EnrollmentListModel>> GetAllAsync(QueryParameters parameters,
    int? studentId = null, int? courseId = null,
    bool includeStudent = false, bool includeCourse = false);
    Task<EnrollmentListModel?> GetByIdAsync(int id);
    Task<EnrollmentListModel> CreateAsync(int studentId, int courseId);
    Task<bool> DeleteAsync(int id);
}