using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.StuPortal.API.Common;
using PRN232.StuPortal.Repositories.Helpers;
using PRN232.StuPortal.Services.Interfaces;
using PRN232.StuPortal.Services.Models.Responses;

namespace PRN232.StuPortal.API.Controllers.V2
{
    [ApiController]
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
            => _studentService = studentService;

        [HttpGet(Name = "GetAllStudentsV2")]
        public async Task<IActionResult> GetAll([FromQuery] QueryParameters query)
        {
            var result = await _studentService.GetAllAsync(query);
            var data = result.Items.Select(MapToV2).ToList();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Students retrieved (API v2)",
                Data = data,
                Pagination = new PaginationMeta
                {
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalItems = result.TotalItems,
                    TotalPages = result.TotalPages
                }
            });
        }

        [HttpGet("{id:int}", Name = "GetStudentByIdV2")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Student not found"
                });

            return Ok(new ApiResponse<StudentV2Response>
            {
                Success = true,
                Message = "Student retrieved (API v2)",
                Data = MapToV2(student)
            });
        }

        private static StudentV2Response MapToV2(StudentResponse student) =>
            new()
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                DisplayLabel = $"{student.FullName} ({student.Email})",
                ApiVersion = "2.0"
            };
    }
}
