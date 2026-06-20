using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.StuPortal.API.Common;
using PRN232.StuPortal.Repositories.Helpers;
using PRN232.StuPortal.Services.Interfaces;
using PRN232.StuPortal.Services.Models.Requests;
using PRN232.StuPortal.Services.Models.Responses;

namespace PRN232.StuPortal.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
            => _studentService = studentService;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryParameters query)
        {
            var result = await _studentService.GetAllAsync(query);

            // -- Fields selection ------------------------------------------
            // ?fields=studentId,fullName,email
            object data;
            if (!string.IsNullOrEmpty(query.Fields))
            {
                var fields = query.Fields.ToLower()
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .ToHashSet();

                data = result.Items.Select(s =>
                {
                    var dict = new Dictionary<string, object?>();
                    if (fields.Contains("studentid")) dict["studentId"] = s.StudentId;
                    if (fields.Contains("fullname")) dict["fullName"] = s.FullName;
                    if (fields.Contains("email")) dict["email"] = s.Email;
                    if (fields.Contains("dateofbirth")) dict["dateOfBirth"] = s.DateOfBirth;
                    return dict;
                });
            }
            else
            {
                data = result.Items;
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Student not found"
                });
            return Ok(new ApiResponse<StudentResponse> { Success = true, Data = student });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStudentRequest request)
        {
            var created = await _studentService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById),
                new { id = created.StudentId },
                new ApiResponse<StudentResponse> { Success = true, Data = created });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateStudentRequest request)
        {
            var updated = await _studentService.UpdateAsync(id, request);
            if (!updated)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Student not found"
                });
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Updated successfully"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _studentService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Student not found"
                });
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Deleted successfully"
            });
        }
    }
}
