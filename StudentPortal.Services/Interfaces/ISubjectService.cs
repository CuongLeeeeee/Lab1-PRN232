using StudentPortal.Repositories.Common;
using StudentPortal.Services.Models;

namespace StudentPortal.Services.Interfaces;

public interface ISubjectService
{
    Task<PagedResult<SubjectModel>> GetAllAsync(QueryParameters parameters, bool includeCourses = false);
    Task<SubjectModel?> GetByIdAsync(int id);
    Task<SubjectModel> CreateAsync(SubjectModel model);
    Task<SubjectModel?> UpdateAsync(int id, SubjectModel model);
    Task<bool> DeleteAsync(int id);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}
