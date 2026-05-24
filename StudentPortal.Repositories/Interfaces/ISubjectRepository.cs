using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Entities;

namespace StudentPortal.Repositories.Interfaces;

public interface ISubjectRepository : IGenericRepository<Subject>
{
    Task<Subject?> GetByCodeAsync(string code);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
    Task<PagedResult<Subject>> SearchAsync(QueryParameters parameters, bool includeCourses = false);
}
