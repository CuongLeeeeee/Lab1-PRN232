using StudentPortal.Repositories.Common;
using StudentPortal.Services.Models;

namespace StudentPortal.Services.Interfaces;

public interface ICourseService
{
    Task<PagedResult<CourseModel>> GetAllAsync(QueryParameters parameters, int? semesterId = null, bool includeSubjects = false);
    Task<CourseModel?> GetByIdAsync(int id);
    Task<CourseModel?> GetByIdWithDetailsAsync(int id);
    Task<CourseModel> CreateAsync(CourseModel model);
    Task<CourseModel?> UpdateAsync(int id, CourseModel model);
    Task<bool> DeleteAsync(int id);
}
