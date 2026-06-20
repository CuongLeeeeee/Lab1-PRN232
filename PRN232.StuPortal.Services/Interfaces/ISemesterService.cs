using PRN232.StuPortal.Repositories.Helpers;
using PRN232.StuPortal.Services.Models.Requests;
using PRN232.StuPortal.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.StuPortal.Services.Interfaces
{
    public interface ISemesterService
    {
        Task<PagedResult<SemesterResponse>> GetAllAsync(QueryParameters query);
        Task<SemesterResponse?> GetByIdAsync(int id);
        Task<SemesterResponse> CreateAsync(CreateSemesterRequest request);
        Task<bool> UpdateAsync(int id, UpdateSemesterRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
