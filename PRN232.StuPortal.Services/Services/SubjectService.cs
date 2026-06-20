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
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _repo;
        public SubjectService(ISubjectRepository repo) => _repo = repo;

        public async Task<PagedResult<SubjectResponse>> GetAllAsync(QueryParameters query)
        {
            var result = await _repo.GetAllAsync(query);
            return new PagedResult<SubjectResponse>
            {
                Items = result.Items.Select(x => new SubjectResponse
                {
                    SubjectId = x.SubjectId,
                    SubjectCode = x.SubjectCode,
                    SubjectName = x.SubjectName,
                    Credit = x.Credit
                }).ToList(),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            };
        }

        public async Task<SubjectResponse?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            if (x == null) return null;
            return new SubjectResponse
            {
                SubjectId = x.SubjectId,
                SubjectCode = x.SubjectCode,
                SubjectName = x.SubjectName,
                Credit = x.Credit
            };
        }

        public async Task<SubjectResponse> CreateAsync(CreateSubjectRequest request)
        {
            var created = await _repo.CreateAsync(new SubjectBusinessModel
            {
                SubjectCode = request.SubjectCode,
                SubjectName = request.SubjectName,
                Credit = request.Credit
            });
            return new SubjectResponse
            {
                SubjectId = created.SubjectId,
                SubjectCode = created.SubjectCode,
                SubjectName = created.SubjectName,
                Credit = created.Credit
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateSubjectRequest request)
            => await _repo.UpdateAsync(new SubjectBusinessModel
            {
                SubjectId = id,
                SubjectCode = request.SubjectCode,
                SubjectName = request.SubjectName,
                Credit = request.Credit
            });

        public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);
    }
}
