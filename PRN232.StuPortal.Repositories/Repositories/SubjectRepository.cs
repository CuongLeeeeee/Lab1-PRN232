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
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ApplicationDbContext _context;
        public SubjectRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<PagedResult<SubjectBusinessModel>> GetAllAsync(QueryParameters query)
        {
            var q = _context.Subjects.AsQueryable();

            if (!string.IsNullOrEmpty(query.Search))
                q = q.Where(x =>
                    x.SubjectName.Contains(query.Search) ||
                    x.SubjectCode.Contains(query.Search));

            q = query.Sort?.ToLower() switch
            {
                "subjectname" => q.OrderBy(x => x.SubjectName),
                "-subjectname" => q.OrderByDescending(x => x.SubjectName),
                "credit" => q.OrderBy(x => x.Credit),
                "-credit" => q.OrderByDescending(x => x.Credit),
                _ => q.OrderBy(x => x.SubjectId)
            };

            var totalItems = await q.CountAsync();
            var items = await q
                .Skip((query.Page - 1) * query.Size)
                .Take(query.Size)
                .Select(x => new SubjectBusinessModel
                {
                    SubjectId = x.SubjectId,
                    SubjectCode = x.SubjectCode,
                    SubjectName = x.SubjectName,
                    Credit = x.Credit
                })
                .ToListAsync();

            return new PagedResult<SubjectBusinessModel>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.Size,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.Size)
            };
        }

        public async Task<SubjectBusinessModel?> GetByIdAsync(int id)
        {
            var s = await _context.Subjects.FirstOrDefaultAsync(x => x.SubjectId == id);
            if (s == null) return null;
            return new SubjectBusinessModel
            {
                SubjectId = s.SubjectId,
                SubjectCode = s.SubjectCode,
                SubjectName = s.SubjectName,
                Credit = s.Credit
            };
        }

        public async Task<SubjectBusinessModel> CreateAsync(SubjectBusinessModel model)
        {
            var entity = new Subject
            {
                SubjectCode = model.SubjectCode,
                SubjectName = model.SubjectName,
                Credit = model.Credit
            };
            _context.Subjects.Add(entity);
            await _context.SaveChangesAsync();
            model.SubjectId = entity.SubjectId;
            return model;
        }

        public async Task<bool> UpdateAsync(SubjectBusinessModel model)
        {
            var entity = await _context.Subjects.FirstOrDefaultAsync(x => x.SubjectId == model.SubjectId);
            if (entity == null) return false;
            entity.SubjectCode = model.SubjectCode;
            entity.SubjectName = model.SubjectName;
            entity.Credit = model.Credit;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Subjects.FirstOrDefaultAsync(x => x.SubjectId == id);
            if (entity == null) return false;
            _context.Subjects.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
