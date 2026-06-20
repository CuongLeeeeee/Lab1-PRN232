using Microsoft.AspNetCore.Mvc;
using PRN232.StuPortal.API.Common;
using PRN232.StuPortal.Repositories.Helpers;
using PRN232.StuPortal.Services.Interfaces;
using PRN232.StuPortal.Services.Models.Requests;
using PRN232.StuPortal.Services.Models.Responses;
using PRN232.StuPortal.Services.Services;

namespace PRN232.StuPortal.API.Controllers
{
    [ApiController]
    [Route("api/courses")]
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

        // -- GET /api/courses ----------------------------------------------
        [HttpGet]
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

        // -- GET /api/courses/{id} -----------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
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

        // -- GET /api/courses/{id}/enrollments?expand=student --------------
        [HttpGet("{id}/enrollments")]
        public async Task<IActionResult> GetEnrollments(
            int id,
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

        // -- POST /api/courses ---------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseRequest request)
        {
            var created = await _courseService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById),
                new { id = created.CourseId },
                new ApiResponse<CourseResponse> { Success = true, Data = created });
        }

        // -- PUT /api/courses/{id} -----------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCourseRequest request)
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

        // -- DELETE /api/courses/{id} --------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
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
