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
    public interface ICourseService
    {
        Task<PagedResult<CourseResponse>> GetAllAsync(QueryParameters query);
        Task<CourseResponse?> GetByIdAsync(int id);
        Task<CourseResponse> CreateAsync(CreateCourseRequest request);
        Task<bool> UpdateAsync(int id, UpdateCourseRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
