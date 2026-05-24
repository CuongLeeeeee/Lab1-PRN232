using Microsoft.EntityFrameworkCore;
using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Context;
using StudentPortal.Repositories.Entities;
using StudentPortal.Repositories.Interfaces;

namespace StudentPortal.Repositories.Implementations;

public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Subject?> GetByCodeAsync(string code)
        => await _context.Subjects
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SubjectCode == code);

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var query = _context.Subjects.Where(s => s.SubjectCode == code);
        if (excludeId.HasValue)
            query = query.Where(s => s.SubjectId != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<PagedResult<Subject>> SearchAsync(QueryParameters parameters, bool includeCourses = false)
    {
        var query = _context.Subjects.AsNoTracking();

        if (includeCourses)
            query = query
                .Include(s => s.CourseSubjects)
                    .ThenInclude(cs => cs.Course)
                        .ThenInclude(c => c.Semester);

        if (!string.IsNullOrWhiteSpace(parameters.Search))
            query = query.Where(s =>
                s.SubjectName.Contains(parameters.Search) ||
                s.SubjectCode.Contains(parameters.Search));

        query = parameters.SortBy?.ToLower() switch
        {
            "code" => parameters.SortDescending ? query.OrderByDescending(s => s.SubjectCode) : query.OrderBy(s => s.SubjectCode),
            "name" => parameters.SortDescending ? query.OrderByDescending(s => s.SubjectName) : query.OrderBy(s => s.SubjectName),
            "credit" => parameters.SortDescending ? query.OrderByDescending(s => s.Credit) : query.OrderBy(s => s.Credit),
            _ => query.OrderBy(s => s.SubjectId)
        };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<Subject>
        {
            Items = items,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }
}
