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
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository
            _studentRepository;

        public StudentService(
            IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<PagedResult<StudentResponse>>GetAllAsync(QueryParameters query)
        {
            var result =
                await _studentRepository .GetAllAsync(query);

            return new PagedResult<StudentResponse>
            {
                Items = result.Items.Select(x =>
                    new StudentResponse
                    {
                        StudentId = x.StudentId,
                        FullName = x.FullName,
                        Email = x.Email,
                        DateOfBirth = x.DateOfBirth
                    }),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            };
        }

        public async Task<StudentResponse?>GetByIdAsync(int id)
        {
            var student =
                await _studentRepository.GetByIdAsync(id);

            if (student == null)
                return null;

            return new StudentResponse
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };
        }

        public async Task<StudentResponse>CreateAsync(CreateStudentRequest request)
        {
            var businessModel =new StudentBusinessModel
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    DateOfBirth = request.DateOfBirth
                };

            var created =
                await _studentRepository.CreateAsync(businessModel);

            return new StudentResponse
            {
                StudentId = created.StudentId,
                FullName = created.FullName,
                Email = created.Email,
                DateOfBirth = created.DateOfBirth
            };
        }

        public async Task<bool> UpdateAsync(
                int id,
                UpdateStudentRequest request)
        {
            var businessModel =
                new StudentBusinessModel
                {
                    StudentId = id,
                    FullName = request.FullName,
                    Email = request.Email,
                    DateOfBirth = request.DateOfBirth
                };

            return await _studentRepository.UpdateAsync(businessModel);
        }

        public async Task<bool>DeleteAsync(int id)
        {
            return await _studentRepository.DeleteAsync(id);
        }
    }
}
