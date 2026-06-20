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
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;
        public EnrollmentRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<PagedResult<EnrollmentBusinessModel>> GetAllAsync(QueryParameters query)
        {
            var q = _context.Enrollments.AsQueryable();

            // Search by status
            if (!string.IsNullOrEmpty(query.Search))
                q = q.Where(x => x.Status.Contains(query.Search));

            // Expand
            var expandList = (query.Expand ?? "").ToLower().Split(',',
                StringSplitOptions.RemoveEmptyEntries);
            bool expandStudent = expandList.Contains("student");
            bool expandCourse = expandList.Contains("course");
            if (expandStudent) q = q.Include(x => x.Student);
            if (expandCourse) q = q.Include(x => x.Course);

            // Sort
            q = query.Sort?.ToLower() switch
            {
                "enrolldate" => q.OrderBy(x => x.EnrollDate),
                "-enrolldate" => q.OrderByDescending(x => x.EnrollDate),
                "status" => q.OrderBy(x => x.Status),
                "-status" => q.OrderByDescending(x => x.Status),
                _ => q.OrderBy(x => x.EnrollmentId)
            };

            var totalItems = await q.CountAsync();
            var items = await q
                .Skip((query.Page - 1) * query.Size)
                .Take(query.Size)
                .ToListAsync();

            var result = items.Select(x => new EnrollmentBusinessModel
            {
                EnrollmentId = x.EnrollmentId,
                StudentId = x.StudentId,
                CourseId = x.CourseId,
                EnrollDate = x.EnrollDate,
                Status = x.Status,
                Student = expandStudent && x.Student != null
                    ? new StudentBusinessModel
                    {
                        StudentId = x.Student.StudentId,
                        FullName = x.Student.FullName,
                        Email = x.Student.Email,
                        DateOfBirth = x.Student.DateOfBirth
                    }
                    : null,
                Course = expandCourse && x.Course != null
                    ? new CourseBusinessModel
                    {
                        CourseId = x.Course.CourseId,
                        CourseName = x.Course.CourseName,
                        SemesterId = x.Course.SemesterId
                    }
                    : null
            }).ToList();

            return new PagedResult<EnrollmentBusinessModel>
            {
                Items = result,
                Page = query.Page,
                PageSize = query.Size,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.Size)
            };
        }

        public async Task<EnrollmentBusinessModel?> GetByIdAsync(int id)
        {
            var e = await _context.Enrollments
                .Include(x => x.Student)
                .Include(x => x.Course)
                .FirstOrDefaultAsync(x => x.EnrollmentId == id);
            if (e == null) return null;
            return new EnrollmentBusinessModel
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollDate = e.EnrollDate,
                Status = e.Status,
                Student = new StudentBusinessModel
                {
                    StudentId = e.Student.StudentId,
                    FullName = e.Student.FullName,
                    Email = e.Student.Email,
                    DateOfBirth = e.Student.DateOfBirth
                },
                Course = new CourseBusinessModel
                {
                    CourseId = e.Course.CourseId,
                    CourseName = e.Course.CourseName,
                    SemesterId = e.Course.SemesterId
                }
            };
        }

        public async Task<EnrollmentBusinessModel> CreateAsync(EnrollmentBusinessModel model)
        {
            var entity = new Enrollment
            {
                StudentId = model.StudentId,
                CourseId = model.CourseId,
                EnrollDate = model.EnrollDate,
                Status = model.Status
            };
            _context.Enrollments.Add(entity);
            await _context.SaveChangesAsync();
            model.EnrollmentId = entity.EnrollmentId;
            return model;
        }

        public async Task<bool> UpdateAsync(EnrollmentBusinessModel model)
        {
            var entity = await _context.Enrollments.FirstOrDefaultAsync(x => x.EnrollmentId == model.EnrollmentId);
            if (entity == null) return false;
            entity.Status = model.Status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Enrollments.FirstOrDefaultAsync(x => x.EnrollmentId == id);
            if (entity == null) return false;
            _context.Enrollments.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<PagedResult<EnrollmentBusinessModel>?> GetByCourseIdAsync(
           int courseId, QueryParameters query)
        {
            // Ki?m tra course có t?n t?i không
            var courseExists = await _context.Courses
                .AnyAsync(x => x.CourseId == courseId);
            if (!courseExists) return null;

            var q = _context.Enrollments
                .Where(x => x.CourseId == courseId)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(query.Search))
                q = q.Where(x => x.Status.Contains(query.Search));

            // Expand
            var expandList = (query.Expand ?? "").ToLower()
                .Split(',', StringSplitOptions.RemoveEmptyEntries);
            bool expandStudent = expandList.Contains("student");
            bool expandCourse = expandList.Contains("course");

            if (expandStudent) q = q.Include(x => x.Student);
            if (expandCourse) q = q.Include(x => x.Course);

            // Sort
            q = query.Sort?.ToLower() switch
            {
                "enrolldate" => q.OrderBy(x => x.EnrollDate),
                "-enrolldate" => q.OrderByDescending(x => x.EnrollDate),
                "status" => q.OrderBy(x => x.Status),
                "-status" => q.OrderByDescending(x => x.Status),
                _ => q.OrderBy(x => x.EnrollmentId)
            };

            var totalItems = await q.CountAsync();
            var rawItems = await q
                .Skip((query.Page - 1) * query.Size)
                .Take(query.Size)
                .ToListAsync();

            return new PagedResult<EnrollmentBusinessModel>
            {
                Items = rawItems.Select(x => MapToBusinessModel(x, expandStudent, expandCourse)),
                Page = query.Page,
                PageSize = query.Size,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.Size)
            };
        }

        // -- HELPER MAPPER -------------------------------------------------

        private static EnrollmentBusinessModel MapToBusinessModel(
          Enrollment x, bool includeStudent, bool includeCourse) =>
          new()
          {
              EnrollmentId = x.EnrollmentId,
              StudentId = x.StudentId,
              CourseId = x.CourseId,
              EnrollDate = x.EnrollDate,
              Status = x.Status,
              Student = includeStudent && x.Student != null
                  ? new StudentBusinessModel
                  {
                      StudentId = x.Student.StudentId,
                      FullName = x.Student.FullName,
                      Email = x.Student.Email,
                      DateOfBirth = x.Student.DateOfBirth
                  }
                  : null,
              Course = includeCourse && x.Course != null
                  ? new CourseBusinessModel
                  {
                      CourseId = x.Course.CourseId,
                      CourseName = x.Course.CourseName,
                      SemesterId = x.Course.SemesterId
                  }
                  : null
          };
    }
}
