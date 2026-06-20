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
    public interface IEnrollmentService
    {
        Task<PagedResult<EnrollmentResponse>> GetAllAsync(QueryParameters query);
        Task<EnrollmentResponse?> GetByIdAsync(int id);
        Task<EnrollmentResponse> CreateAsync(CreateEnrollmentRequest request);
        Task<bool> UpdateAsync(int id, UpdateEnrollmentRequest request);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<EnrollmentResponse>?> GetByCourseIdAsync(int courseId, QueryParameters query);

    }
}
