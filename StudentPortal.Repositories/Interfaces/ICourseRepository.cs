using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Entities;

namespace StudentPortal.Repositories.Interfaces;

public interface ICourseRepository : IGenericRepository<Course>
{
    Task<Course?> GetByIdWithDetailsAsync(int id);
    Task<PagedResult<Course>> SearchAsync(QueryParameters parameters, int? semesterId = null, bool includeSubjects = false);
    Task<IEnumerable<Course>> GetBySemesterAsync(int semesterId);
}
