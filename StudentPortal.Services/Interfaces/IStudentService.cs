using StudentPortal.Repositories.Common;
using StudentPortal.Services.Models;

namespace StudentPortal.Services.Interfaces;

public interface IStudentService
{
    Task<PagedResult<StudentModel>> GetAllAsync(QueryParameters parameters, bool includeCourses = false);
    Task<StudentModel?> GetByIdAsync(int id);
    Task<StudentModel?> GetByIdWithEnrollmentsAsync(int id);
    Task<StudentModel> CreateAsync(StudentModel model);
    Task<StudentModel?> UpdateAsync(int id, StudentModel model);
    Task<bool> DeleteAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}
