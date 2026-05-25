using Microsoft.EntityFrameworkCore;
using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Context;
using StudentPortal.Repositories.Entities;
using StudentPortal.Repositories.Interfaces;

namespace StudentPortal.Repositories.Implementations;

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Student?> GetByEmailAsync(string email)
        => await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Email == email);

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var query = _context.Students.Where(s => s.Email == email);
        if (excludeId.HasValue)
            query = query.Where(s => s.StudentId != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<Student?> GetByIdWithEnrollmentsAsync(int id)
        => await _context.Students
            .Include(s => s.Enrollments).ThenInclude(e => e.Course).ThenInclude(c => c.Semester)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentId == id);

    public async Task<PagedResult<Student>> SearchAsync(QueryParameters parameters, bool includeEnrollments = false)
    {
        var query = _context.Students.AsNoTracking();

        if (includeEnrollments)
            query = query
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Semester);

        if (!string.IsNullOrWhiteSpace(parameters.Search))
            query = query.Where(s =>
                s.FullName.Contains(parameters.Search) ||
                s.Email.Contains(parameters.Search));

        query = parameters.SortBy?.ToLower() switch
        {
            "name" => parameters.SortDescending ? query.OrderByDescending(s => s.FullName) : query.OrderBy(s => s.FullName),
            "email" => parameters.SortDescending ? query.OrderByDescending(s => s.Email) : query.OrderBy(s => s.Email),
            _ => query.OrderBy(s => s.StudentId)
        };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<Student>
        {
            Items = items,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }
}
