using StudentPortal.Repositories.Common;
using StudentPortal.Services.Models;

namespace StudentPortal.Services.Interfaces;

public interface ISemesterService
{
    Task<PagedResult<SemesterModel>> GetAllAsync(QueryParameters parameters, bool includeCourses = false);
    Task<SemesterModel?> GetByIdAsync(int id);
    Task<SemesterModel?> GetByIdWithCoursesAsync(int id);
    Task<SemesterModel> CreateAsync(SemesterModel model);
    Task<SemesterModel?> UpdateAsync(int id, SemesterModel model);
    Task<bool> DeleteAsync(int id);
}
