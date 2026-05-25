using Microsoft.AspNetCore.Mvc;
using StudentPortal.API.DTOs.Request;
using StudentPortal.API.DTOs.Response;
using StudentPortal.API.Extensions;
using StudentPortal.Repositories.Common;
using StudentPortal.Services.Interfaces;
using static StudentPortal.API.DTOs.Response.SelectProjector;

namespace StudentPortal.API.Controllers;

/// <summary>CRUD operations for Enrollments.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;
    private readonly ILogger<EnrollmentsController> _logger;

    public EnrollmentsController(IEnrollmentService service, ILogger<EnrollmentsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>Get all enrollments with pagination, filtering, $select and $expand.</summary>
    /// <param name="request">Pagination, filter and projection parameters.</param>
    /// <response code="200">Returns a paged list of enrollments.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<object>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationRequest request)
    {
        var parameters = new QueryParameters
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Search = request.Search,
            SortBy = request.SortBy,
            SortDescending = request.SortDescending,
            Select = request.Select,
            Expand = request.Expand
        };

        var expandedRels = parameters.GetExpandedRelations().ToList();
        var selectedFields = parameters.GetSelectedFields().ToList();
        var includeStudent = expandedRels.Contains("student");
        var includeCourse = expandedRels.Contains("course");

        var paged = await _service.GetAllAsync(
            parameters, null,null, includeStudent, includeCourse);

        IEnumerable<object> items;
        if (expandedRels.Any())
            items = paged.Items.Select(m => (object)m.ToExpandedResponse(expandedRels));
        else
            items = paged.Items.Select(m => (object)m.ToResponse());

        if (selectedFields.Any())
            items = SelectProjector.ProjectMany(items, selectedFields);

        var response = new PagedResponse<object>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize,
            TotalPages = paged.TotalPages,
            HasPreviousPage = paged.HasPreviousPage,
            HasNextPage = paged.HasNextPage
        };

        return Ok(ApiResponse<PagedResponse<object>>.Ok(response,
            $"Retrieved {response.TotalCount} enrollment(s)."));
    }

    /// <summary>Get a single enrollment by ID.</summary>
    /// <param name="id">Enrollment ID.</param>
    /// <response code="200">Enrollment found.</response>
    /// <response code="404">Enrollment not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentBaseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _service.GetByIdAsync(id);
        if (model is null)
            return NotFound(ApiResponse<object>.Fail($"Enrollment with ID {id} was not found."));

        return Ok(ApiResponse<EnrollmentBaseResponse>.Ok(model.ToResponse()));
    }

    /// <summary>Enroll a student into a course.</summary>
    /// <param name="request">StudentId and CourseId.</param>
    /// <response code="201">Enrollment created successfully.</response>
    /// <response code="400">Validation failed, student/course not found, or already enrolled.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentBaseResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateEnrollmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation failed.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

        try
        {
            var created = await _service.CreateAsync(request.StudentId, request.CourseId);
            return CreatedAtAction(nameof(GetById), new { id = created.EnrollmentId },
                ApiResponse<EnrollmentBaseResponse>.Ok(
                    created.ToResponse(), "Enrollment created successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>Delete an enrollment by ID (unenroll a student).</summary>
    /// <param name="id">Enrollment ID.</param>
    /// <response code="200">Enrollment deleted.</response>
    /// <response code="404">Enrollment not found.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Enrollment with ID {id} was not found."));

        return Ok(ApiResponse<object>.Ok(new { }, "Enrollment deleted successfully."));
    }
}