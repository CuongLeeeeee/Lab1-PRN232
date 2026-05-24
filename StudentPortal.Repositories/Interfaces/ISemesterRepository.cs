using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Entities;

namespace StudentPortal.Repositories.Interfaces;

public interface ISemesterRepository : IGenericRepository<Semester>
{
    Task<Semester?> GetByIdWithCoursesAsync(int id);
    Task<PagedResult<Semester>> SearchAsync(QueryParameters parameters, bool includeCourses = false);
}
