using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Entities;

namespace StudentPortal.Repositories.Interfaces;

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<Student?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    Task<Student?> GetByIdWithEnrollmentsAsync(int id);
    Task<PagedResult<Student>> SearchAsync(QueryParameters parameters, bool includeCourses = false);
}
