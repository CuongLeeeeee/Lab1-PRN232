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
    public class SemesterRepository : ISemesterRepository
    {
        private readonly ApplicationDbContext _context;
        public SemesterRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<PagedResult<SemesterBusinessModel>> GetAllAsync(QueryParameters query)
        {
            var q = _context.Semesters.AsQueryable();

            if (!string.IsNullOrEmpty(query.Search))
                q = q.Where(x => x.SemesterName.Contains(query.Search));

            q = query.Sort?.ToLower() switch
            {
                "semestername" => q.OrderBy(x => x.SemesterName),
                "-semestername" => q.OrderByDescending(x => x.SemesterName),
                "startdate" => q.OrderBy(x => x.StartDate),
                "-startdate" => q.OrderByDescending(x => x.StartDate),
                _ => q.OrderBy(x => x.SemesterId)
            };

            var totalItems = await q.CountAsync();
            var items = await q
                .Skip((query.Page - 1) * query.Size)
                .Take(query.Size)
                .Select(x => new SemesterBusinessModel
                {
                    SemesterId = x.SemesterId,
                    SemesterName = x.SemesterName,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                })
                .ToListAsync();

            return new PagedResult<SemesterBusinessModel>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.Size,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.Size)
            };
        }

        public async Task<SemesterBusinessModel?> GetByIdAsync(int id)
        {
            var s = await _context.Semesters.FirstOrDefaultAsync(x => x.SemesterId == id);
            if (s == null) return null;
            return new SemesterBusinessModel
            {
                SemesterId = s.SemesterId,
                SemesterName = s.SemesterName,
                StartDate = s.StartDate,
                EndDate = s.EndDate
            };
        }

        public async Task<SemesterBusinessModel> CreateAsync(SemesterBusinessModel model)
        {
            var entity = new Semester
            {
                SemesterName = model.SemesterName,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };
            _context.Semesters.Add(entity);
            await _context.SaveChangesAsync();
            model.SemesterId = entity.SemesterId;
            return model;
        }

        public async Task<bool> UpdateAsync(SemesterBusinessModel model)
        {
            var entity = await _context.Semesters.FirstOrDefaultAsync(x => x.SemesterId == model.SemesterId);
            if (entity == null) return false;
            entity.SemesterName = model.SemesterName;
            entity.StartDate = model.StartDate;
            entity.EndDate = model.EndDate;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Semesters.FirstOrDefaultAsync(x => x.SemesterId == id);
            if (entity == null) return false;
            _context.Semesters.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
