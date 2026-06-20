using PRN232.StuPortal.Repositories.BusinessModels;
using PRN232.StuPortal.Repositories.Entities;
using PRN232.StuPortal.Repositories.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.StuPortal.Repositories.Interfaces
{
    public interface IStudentRepository
    {

        Task<PagedResult<StudentBusinessModel>> GetAllAsync(QueryParameters query);

        Task<StudentBusinessModel?> GetByIdAsync(int id);

        Task<StudentBusinessModel> CreateAsync(StudentBusinessModel model);

        Task<bool> UpdateAsync(StudentBusinessModel model);

        Task<bool> DeleteAsync(int id);
    }
}
