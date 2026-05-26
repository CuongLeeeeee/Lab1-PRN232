using Microsoft.EntityFrameworkCore;
using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Context;
using StudentPortal.Repositories.Entities;
using StudentPortal.Repositories.Interfaces;

namespace StudentPortal.Repositories.Implementations;

public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Course?> GetByIdWithDetailsAsync(int id)
        => await _context.Courses
            .Include(c => c.Semester)
            .Include(c => c.CourseSubjects).ThenInclude(cs => cs.Subject)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CourseId == id);

    public async Task<IEnumerable<Course>> GetBySemesterAsync(int semesterId)
        => await _context.Courses
            .Where(c => c.SemesterId == semesterId)
            .Include(c => c.Semester)
            .AsNoTracking()
            .ToListAsync();

    public async Task<PagedResult<Course>> SearchAsync(QueryParameters parameters, int? semesterId = null, bool includeSubjects = false)
    {
        var query = _context.Courses
            .Include(c => c.Semester)
            .AsNoTracking();

        if (includeSubjects)
            query = query.Include(c => c.CourseSubjects).ThenInclude(cs => cs.Subject);

        if (semesterId.HasValue)
            query = query.Where(c => c.SemesterId == semesterId.Value);

        if (!string.IsNullOrWhiteSpace(parameters.Search))
            query = query.Where(c => c.CourseName.Contains(parameters.Search));

        query = parameters.Sort?.ToLower() switch
        {
            "name" => query.OrderBy(c => c.CourseName),
            "-name" => query.OrderByDescending(c => c.CourseName),
            "semester" => query.OrderBy(c => c.Semester.SemesterName),
            "-semester" => query.OrderByDescending(c => c.Semester.SemesterName),
            _ => query.OrderBy(c => c.CourseId)
        };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<Course>
        {
            Items = items,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }
}
