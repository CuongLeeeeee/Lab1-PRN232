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
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<PagedResult<StudentBusinessModel>>
            GetAllAsync(QueryParameters query)
        {
            var q = _context.Students.AsQueryable();

            // -- Search --------------------------------------------------
            if (!string.IsNullOrEmpty(query.Search))
                q = q.Where(x =>
                    x.FullName.Contains(query.Search) ||
                    x.Email.Contains(query.Search));

            // -- Sort ----------------------------------------------------
            q = query.Sort?.ToLower() switch
            {
                "fullname" => q.OrderBy(x => x.FullName),
                "-fullname" => q.OrderByDescending(x => x.FullName),
                "email" => q.OrderBy(x => x.Email),
                "-email" => q.OrderByDescending(x => x.Email),
                "dateofbirth" => q.OrderBy(x => x.DateOfBirth),
                "-dateofbirth" => q.OrderByDescending(x => x.DateOfBirth),
                _ => q.OrderBy(x => x.StudentId)
            };

            var totalItems = await q.CountAsync();

            var students = await q
                .Skip((query.Page - 1) * query.Size)
                .Take(query.Size)
                .Select(x => new StudentBusinessModel
                {
                    StudentId = x.StudentId,
                    FullName = x.FullName,
                    Email = x.Email,
                    DateOfBirth = x.DateOfBirth
                })
                .ToListAsync();

            return new PagedResult<StudentBusinessModel>
            {
                Items = students,
                Page = query.Page,
                PageSize = query.Size,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.Size)
            };
        }

        public async Task<StudentBusinessModel?> GetByIdAsync(int id)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(x => x.StudentId == id);

            if (student == null) return null;

            return new StudentBusinessModel
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };
        }

        public async Task<StudentBusinessModel> CreateAsync(StudentBusinessModel model)
        {
            var entity = new Student
            {
                FullName = model.FullName,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth
            };
            _context.Students.Add(entity);
            await _context.SaveChangesAsync();
            model.StudentId = entity.StudentId;
            return model;
        }

        public async Task<bool> UpdateAsync(StudentBusinessModel model)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(x => x.StudentId == model.StudentId);
            if (student == null) return false;
            student.FullName = model.FullName;
            student.Email = model.Email;
            student.DateOfBirth = model.DateOfBirth;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(x => x.StudentId == id);
            if (student == null) return false;
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
