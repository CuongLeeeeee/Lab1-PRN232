using PRN232.StuPortal.Repositories.BusinessModels;
using PRN232.StuPortal.Repositories.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.StuPortal.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<PagedResult<CourseBusinessModel>> GetAllAsync(QueryParameters query);
        Task<CourseBusinessModel?> GetByIdAsync(int id);
        Task<CourseBusinessModel> CreateAsync(CourseBusinessModel model);
        Task<bool> UpdateAsync(CourseBusinessModel model);
        Task<bool> DeleteAsync(int id);
    }
}
