using PRN232.StuPortal.Repositories.BusinessModels;
using PRN232.StuPortal.Repositories.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.StuPortal.Repositories.Interfaces
{
    public interface ISemesterRepository
    {
        Task<PagedResult<SemesterBusinessModel>> GetAllAsync(QueryParameters query);
        Task<SemesterBusinessModel?> GetByIdAsync(int id);
        Task<SemesterBusinessModel> CreateAsync(SemesterBusinessModel model);
        Task<bool> UpdateAsync(SemesterBusinessModel model);
        Task<bool> DeleteAsync(int id);
    }
}
