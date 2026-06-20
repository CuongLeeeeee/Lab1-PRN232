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
    public interface IStudentService
    {
       
        
            Task<PagedResult<StudentResponse>> GetAllAsync(QueryParameters query);

            Task<StudentResponse?>GetByIdAsync(int id);

            Task<StudentResponse>CreateAsync(CreateStudentRequest request);

            Task<bool>UpdateAsync(int id,UpdateStudentRequest request);

            Task<bool>DeleteAsync(int id);
        
    }
}
