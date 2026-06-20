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
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repo;
        public CourseService(ICourseRepository repo) => _repo = repo;

        public async Task<PagedResult<CourseResponse>> GetAllAsync(QueryParameters query)
        {
            var result = await _repo.GetAllAsync(query);
            return new PagedResult<CourseResponse>
            {
                Items = result.Items.Select(x => new CourseResponse
                {
                    CourseId = x.CourseId,
                    CourseName = x.CourseName,
                    SemesterId = x.SemesterId,
                    Semester = x.SemesterName != null
                        ? new SemesterResponse { SemesterName = x.SemesterName }
                        : null
                }),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            };
        }

        public async Task<CourseResponse?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            if (x == null) return null;
            return new CourseResponse
            {
                CourseId = x.CourseId,
                CourseName = x.CourseName,
                SemesterId = x.SemesterId,
                Semester = x.SemesterName != null
                    ? new SemesterResponse { SemesterName = x.SemesterName }
                    : null
            };
        }

        public async Task<CourseResponse> CreateAsync(CreateCourseRequest request)
        {
            var created = await _repo.CreateAsync(new CourseBusinessModel
            {
                CourseName = request.CourseName,
                SemesterId = request.SemesterId
            });
            return new CourseResponse
            {
                CourseId = created.CourseId,
                CourseName = created.CourseName,
                SemesterId = created.SemesterId
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateCourseRequest request)
            => await _repo.UpdateAsync(new CourseBusinessModel
            {
                CourseId = id,
                CourseName = request.CourseName,
                SemesterId = request.SemesterId
            });

        public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);
    }
}
