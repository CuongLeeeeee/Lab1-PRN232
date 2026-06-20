using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PRN232.StuPortal.API.Common;
using PRN232.StuPortal.Repositories.Helpers;
using PRN232.StuPortal.Services.Interfaces;
using PRN232.StuPortal.Services.Models.Requests;
using PRN232.StuPortal.Services.Models.Responses;

namespace PRN232.StuPortal.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;

        public CoursesController(
            ICourseService courseService,
            IEnrollmentService enrollmentService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
        }

        [HttpGet(Name = "GetAllCourses")]
        public async Task<IActionResult> GetAll([FromQuery] QueryParameters query)
        {
            var result = await _courseService.GetAllAsync(query);

            object data;
            if (!string.IsNullOrEmpty(query.Fields))
            {
                var fields = query.Fields.ToLower()
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .ToHashSet();

                if (fields.Contains("semester") && string.IsNullOrEmpty(query.Expand))
                    query.Expand = "semester";

                data = result.Items.Select(x =>
                {
                    var dict = new Dictionary<string, object?>();
                    if (fields.Contains("courseid")) dict["courseId"] = x.CourseId;
                    if (fields.Contains("coursename")) dict["courseName"] = x.CourseName;
                    if (fields.Contains("semesterid")) dict["semesterId"] = x.SemesterId;
                    if (fields.Contains("semester") && x.Semester != null)
                        dict["semester"] = x.Semester;

                    return dict.Count == 0
                        ? new Dictionary<string, object?>
                        {
                            ["courseId"] = x.CourseId,
                            ["courseName"] = x.CourseName,
                            ["semesterId"] = x.SemesterId
                        }
                        : dict;
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

        [HttpGet("{id:int}", Name = "GetCourseById")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var item = await _courseService.GetByIdAsync(id);
            if (item == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course not found"
                });
            return Ok(new ApiResponse<CourseResponse> { Success = true, Data = item });
        }

        [HttpGet("{courseId:int}/students", Name = "GetCourseStudents")]
        public async Task<IActionResult> GetStudentsByCourse(
            [FromRoute] int courseId,
            [FromQuery] QueryParameters query)
        {
            query.Expand = string.IsNullOrWhiteSpace(query.Expand)
                ? "student"
                : $"{query.Expand},student";

            var result = await _enrollmentService.GetByCourseIdAsync(courseId, query);
            if (result == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course not found"
                });

            var students = result.Items
                .Where(e => e.Student != null)
                .Select(e => e.Student!)
                .GroupBy(s => s.StudentId)
                .Select(g => g.First())
                .ToList();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Students enrolled in course retrieved successfully",
                Data = students,
                Pagination = new PaginationMeta
                {
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalItems = students.Count,
                    TotalPages = result.TotalPages
                }
            });
        }

        [HttpGet("{id:int}/enrollments", Name = "GetCourseEnrollments")]
        public async Task<IActionResult> GetEnrollments(
            [FromRoute] int id,
            [FromQuery] QueryParameters query)
        {
            var result = await _enrollmentService.GetByCourseIdAsync(id, query);

            if (result == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course not found"
                });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Enrollments retrieved successfully",
                Data = result.Items,
                Pagination = new PaginationMeta
                {
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalItems = result.TotalItems,
                    TotalPages = result.TotalPages
                }
            });
        }

        [HttpPost(Name = "CreateCourse")]
        public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
        {
            var created = await _courseService.CreateAsync(request);
            return CreatedAtRoute(
                "GetCourseById",
                new { id = created.CourseId, version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0" },
                new ApiResponse<CourseResponse> { Success = true, Data = created });
        }

        [HttpPut("{id:int}", Name = "UpdateCourse")]
        public async Task<IActionResult> Update(
            [FromRoute] int id,
            [FromBody] UpdateCourseRequest request)
        {
            var updated = await _courseService.UpdateAsync(id, request);
            if (!updated)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course not found"
                });
            return Ok(new ApiResponse<object> { Success = true, Message = "Updated successfully" });
        }

        [HttpDelete("{id:int}", Name = "DeleteCourse")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var deleted = await _courseService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course not found"
                });
            return Ok(new ApiResponse<object> { Success = true, Message = "Deleted successfully" });
        }
    }
}
