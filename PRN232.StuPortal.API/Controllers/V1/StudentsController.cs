using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.StuPortal.API.Common;
using PRN232.StuPortal.Repositories.Helpers;
using PRN232.StuPortal.Services.Interfaces;
using PRN232.StuPortal.Services.Models.Requests;
using PRN232.StuPortal.Services.Models.Responses;

namespace PRN232.StuPortal.API.Controllers.V1
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(
            IStudentService studentService,
            ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        [HttpGet(Name = "GetAllStudents")]
        public async Task<IActionResult> GetAll([FromQuery] QueryParameters query)
        {
            var result = await _studentService.GetAllAsync(query);

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

        [HttpGet("{id:int}", Name = "GetStudentById")]
        public async Task<IActionResult> GetById([FromRoute] int id)
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

        [HttpPost(Name = "CreateStudent")]
        public async Task<IActionResult> Create(
            [FromBody] CreateStudentRequest request,
            [FromHeader(Name = "X-Request-Id")] string? requestId)
        {
            _logger.LogInformation(
                "Create student request received. X-Request-Id: {RequestId}, Email: {Email}",
                requestId ?? "(none)",
                request.Email);

            var created = await _studentService.CreateAsync(request);
            return CreatedAtRoute(
                "GetStudentById",
                new { id = created.StudentId, version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0" },
                new ApiResponse<StudentResponse> { Success = true, Data = created });
        }

        [HttpPut("{id:int}", Name = "UpdateStudent")]
        public async Task<IActionResult> Update(
            [FromRoute] int id,
            [FromBody] UpdateStudentRequest request)
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

        [HttpDelete("{id:int}", Name = "DeleteStudent")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
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
