using Microsoft.EntityFrameworkCore;
using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Context;
using StudentPortal.Repositories.Entities;
using StudentPortal.Repositories.Interfaces;

namespace StudentPortal.Repositories.Implementations;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
        => _context = context;

    public async Task<Enrollment?> GetByIdAsync(int id)
        => await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course).ThenInclude(c => c.Semester)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrollmentId == id);

    public async Task<PagedResult<Enrollment>> SearchAsync(
    QueryParameters parameters,
    int? studentId = null,
    int? courseId = null,
    bool includeStudent = false,
    bool includeCourse = false)
    {
        var query = _context.Enrollments.AsNoTracking();

        if (includeStudent)
            query = query.Include(e => e.Student);

        if (includeCourse)
            query = query
                .Include(e => e.Course);

        if (studentId.HasValue)
            query = query.Where(e => e.StudentId == studentId.Value);

        if (courseId.HasValue)
            query = query.Where(e => e.CourseId == courseId.Value);

        if (!string.IsNullOrWhiteSpace(parameters.Search))
            query = query.Where(e =>
                e.Student.FullName.Contains(parameters.Search) ||
                e.Course.CourseName.Contains(parameters.Search));

        query = parameters.SortBy?.ToLower() switch
        {
            "enrolledat" => parameters.SortDescending ? query.OrderByDescending(e => e.EnrolledAt) : query.OrderBy(e => e.EnrolledAt),
            _ => query.OrderBy(e => e.EnrollmentId)
        };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<Enrollment>
        {
            Items = items,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }

    public async Task<Enrollment> CreateAsync(Enrollment entity)
    {
        await _context.Enrollments.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Enrollments.FindAsync(id);
        if (entity is null) return false;
        _context.Enrollments.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int studentId, int courseId)
        => await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

    public async Task<bool> StudentExistsAsync(int studentId)
        => await _context.Students.AnyAsync(s => s.StudentId == studentId);

    public async Task<bool> CourseExistsAsync(int courseId)
        => await _context.Courses.AnyAsync(c => c.CourseId == courseId);
}