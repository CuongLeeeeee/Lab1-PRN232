using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PRN232.StuPortal.API.Common;
using PRN232.StuPortal.Repositories.Helpers;
using PRN232.StuPortal.Services.Interfaces;
using PRN232.StuPortal.Services.Models.Requests;
using PRN232.StuPortal.Services.Models.Responses;

namespace PRN232.StuPortal.API.Controllers
{
    [ApiController]
    [ApiVersionNeutral]
    [Route("api/semesters")]
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _service;
        public SemestersController(ISemesterService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryParameters query)
        {
            var result = await _service.GetAllAsync(query);

            object data;
            if (!string.IsNullOrEmpty(query.Fields))
            {
                var fields = query.Fields.ToLower()
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .ToHashSet();

                data = result.Items.Select(x =>
                {
                    var dict = new Dictionary<string, object?>();
                    if (fields.Contains("semesterid")) dict["semesterId"] = x.SemesterId;
                    if (fields.Contains("semestername")) dict["semesterName"] = x.SemesterName;
                    if (fields.Contains("startdate")) dict["startDate"] = x.StartDate;
                    if (fields.Contains("enddate")) dict["endDate"] = x.EndDate;
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

        [HttpGet("{id:int}", Name = "GetSemesterById")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Semester not found"
                });
            return Ok(new ApiResponse<SemesterResponse> { Success = true, Data = item });
        }

        [HttpPost(Name = "CreateSemester")]
        public async Task<IActionResult> Create([FromBody] CreateSemesterRequest request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtRoute(
                "GetSemesterById",
                new { id = created.SemesterId },
                new ApiResponse<SemesterResponse> { Success = true, Data = created });
        }

        [HttpPut("{id:int}", Name = "UpdateSemester")]
        public async Task<IActionResult> Update(
            [FromRoute] int id,
            [FromBody] UpdateSemesterRequest request)
        {
            var updated = await _service.UpdateAsync(id, request);
            if (!updated)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Semester not found"
                });
            return Ok(new ApiResponse<object> { Success = true, Message = "Updated successfully" });
        }

        [HttpDelete("{id:int}", Name = "DeleteSemester")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Semester not found"
                });
            return Ok(new ApiResponse<object> { Success = true, Message = "Deleted successfully" });
        }
    }
}
