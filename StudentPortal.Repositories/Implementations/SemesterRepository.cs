using Microsoft.EntityFrameworkCore;
using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Context;
using StudentPortal.Repositories.Entities;
using StudentPortal.Repositories.Interfaces;

namespace StudentPortal.Repositories.Implementations;

public class SemesterRepository : GenericRepository<Semester>, ISemesterRepository
{
    public SemesterRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Semester?> GetByIdWithCoursesAsync(int id)
        => await _context.Semesters
            .Include(s => s.Courses)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SemesterId == id);

    public async Task<PagedResult<Semester>> SearchAsync(QueryParameters parameters, bool includeCourses = false)
    {
        var query = _context.Semesters.AsNoTracking();

        if (includeCourses)
            query = query.Include(s => s.Courses);

        if (!string.IsNullOrWhiteSpace(parameters.Search))
            query = query.Where(s => s.SemesterName.Contains(parameters.Search));

        query = parameters.Sort?.ToLower() switch
        {
            "name" => query.OrderBy(s => s.SemesterName),
            "-name" => query.OrderByDescending(s => s.SemesterName),
            "startdate" => query.OrderBy(s => s.StartDate),
            "-startdate" => query.OrderByDescending(s => s.StartDate),
            "enddate" => query.OrderBy(s => s.EndDate),
            "-enddate" => query.OrderByDescending(s => s.EndDate),
            _ => query.OrderBy(s => s.SemesterId)
        };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<Semester>
        {
            Items = items,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }
}
