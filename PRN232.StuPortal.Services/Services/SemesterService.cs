using PRN232.StuPortal.Repositories.BusinessModels;
using PRN232.StuPortal.Repositories.Helpers;
using PRN232.StuPortal.Repositories.Interfaces;
using PRN232.StuPortal.Services.Interfaces;
using PRN232.StuPortal.Services.Models.Requests;
using PRN232.StuPortal.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.StuPortal.Services.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _repo;
        public SemesterService(ISemesterRepository repo) => _repo = repo;

        public async Task<PagedResult<SemesterResponse>> GetAllAsync(QueryParameters query)
        {
            var result = await _repo.GetAllAsync(query);
            return new PagedResult<SemesterResponse>
            {
                Items = result.Items.Select(x => new SemesterResponse
                {
                    SemesterId = x.SemesterId,
                    SemesterName = x.SemesterName,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                }).ToList(),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            };
        }

        public async Task<SemesterResponse?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            if (x == null) return null;
            return new SemesterResponse
            {
                SemesterId = x.SemesterId,
                SemesterName = x.SemesterName,
                StartDate = x.StartDate,
                EndDate = x.EndDate
            };
        }

        public async Task<SemesterResponse> CreateAsync(CreateSemesterRequest request)
        {
            var created = await _repo.CreateAsync(new SemesterBusinessModel
            {
                SemesterName = request.SemesterName,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            });
            return new SemesterResponse
            {
                SemesterId = created.SemesterId,
                SemesterName = created.SemesterName,
                StartDate = created.StartDate,
                EndDate = created.EndDate
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateSemesterRequest request)
            => await _repo.UpdateAsync(new SemesterBusinessModel
            {
                SemesterId = id,
                SemesterName = request.SemesterName,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            });

        public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);
    }
}
