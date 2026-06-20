using Microsoft.EntityFrameworkCore;
using PRN232.StuPortal.Repositories.BusinessModels;
using PRN232.StuPortal.Repositories.Data;
using PRN232.StuPortal.Repositories.Entities;
using PRN232.StuPortal.Repositories.Helpers;
using PRN232.StuPortal.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.StuPortal.Repositories.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;
        public CourseRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<PagedResult<CourseBusinessModel>> GetAllAsync(QueryParameters query)
        {
            var q = _context.Courses.AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(query.Search))
                q = q.Where(x => x.CourseName.Contains(query.Search));

            // Expand semester
            var expandList = (query.Expand ?? "").ToLower().Split(',',
                StringSplitOptions.RemoveEmptyEntries);
            bool expandSemester = expandList.Contains("semester");
            if (expandSemester)
                q = q.Include(x => x.Semester);

            // Sort
            q = query.Sort?.ToLower() switch
            {
                "coursename" => q.OrderBy(x => x.CourseName),
                "-coursename" => q.OrderByDescending(x => x.CourseName),
                _ => q.OrderBy(x => x.CourseId)
            };

            var totalItems = await q.CountAsync();
            var items = await q
                .Skip((query.Page - 1) * query.Size)
                .Take(query.Size)
                .Select(x => new CourseBusinessModel
                {
                    CourseId = x.CourseId,
                    CourseName = x.CourseName,
                    SemesterId = x.SemesterId,
                    SemesterName = expandSemester ? x.Semester.SemesterName : null
                })
                .ToListAsync();

            return new PagedResult<CourseBusinessModel>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.Size,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.Size)
            };
        }

        public async Task<CourseBusinessModel?> GetByIdAsync(int id)
        {
            var c = await _context.Courses
                .Include(x => x.Semester)
                .FirstOrDefaultAsync(x => x.CourseId == id);
            if (c == null) return null;
            return new CourseBusinessModel
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                SemesterId = c.SemesterId,
                SemesterName = c.Semester?.SemesterName
            };
        }

        public async Task<CourseBusinessModel> CreateAsync(CourseBusinessModel model)
        {
            var entity = new Course { CourseName = model.CourseName, SemesterId = model.SemesterId };
            _context.Courses.Add(entity);
            await _context.SaveChangesAsync();
            model.CourseId = entity.CourseId;
            return model;
        }

        public async Task<bool> UpdateAsync(CourseBusinessModel model)
        {
            var entity = await _context.Courses.FirstOrDefaultAsync(x => x.CourseId == model.CourseId);
            if (entity == null) return false;
            entity.CourseName = model.CourseName;
            entity.SemesterId = model.SemesterId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Courses.FirstOrDefaultAsync(x => x.CourseId == id);
            if (entity == null) return false;
            _context.Courses.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
