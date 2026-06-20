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
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repo;
        public EnrollmentService(IEnrollmentRepository repo) => _repo = repo;

        public async Task<PagedResult<EnrollmentResponse>> GetAllAsync(QueryParameters query)
        {
            var result = await _repo.GetAllAsync(query);
            return new PagedResult<EnrollmentResponse>
            {
                Items = result.Items.Select(Map).ToList(),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            };
        }

        public async Task<EnrollmentResponse?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            return x == null ? null : Map(x);
        }

        public async Task<EnrollmentResponse> CreateAsync(CreateEnrollmentRequest request)
        {
            var created = await _repo.CreateAsync(new EnrollmentBusinessModel
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrollDate = request.EnrollDate,
                Status = request.Status
            });
            return Map(created);
        }

        public async Task<bool> UpdateAsync(int id, UpdateEnrollmentRequest request)
            => await _repo.UpdateAsync(new EnrollmentBusinessModel
            {
                EnrollmentId = id,
                Status = request.Status
            });

        public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

        // -- mapper -------------------------------------------------------
        private static EnrollmentResponse Map(EnrollmentBusinessModel x) =>
            new()
            {
                EnrollmentId = x.EnrollmentId,
                StudentId = x.StudentId,
                CourseId = x.CourseId,
                EnrollDate = x.EnrollDate,
                Status = x.Status,
                Student = x.Student == null ? null : new StudentResponse
                {
                    StudentId = x.Student.StudentId,
                    FullName = x.Student.FullName,
                    Email = x.Student.Email,
                    DateOfBirth = x.Student.DateOfBirth
                },
                Course = x.Course == null ? null : new CourseResponse
                {
                    CourseId = x.Course.CourseId,
                    CourseName = x.Course.CourseName,
                    SemesterId = x.Course.SemesterId
                }
            };
        public async Task<PagedResult<EnrollmentResponse>?> GetByCourseIdAsync(
            int courseId, QueryParameters query)
        {
            var result = await _repo.GetByCourseIdAsync(courseId, query);
            if (result == null) return null;

            return new PagedResult<EnrollmentResponse>
            {
                Items = result.Items.Select(Map).ToList(),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            };
        }

    }
}
