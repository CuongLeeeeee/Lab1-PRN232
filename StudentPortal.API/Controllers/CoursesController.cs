using Microsoft.AspNetCore.Mvc;
using StudentPortal.API.DTOs.Request;
using StudentPortal.API.DTOs.Response;
using StudentPortal.API.Extensions;
using StudentPortal.Repositories.Common;
using StudentPortal.Services.Implementations;
using StudentPortal.Services.Interfaces;

namespace StudentPortal.API.Controllers;

/// <summary>CRUD operations for Courses.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
private readonly IEnrollmentService _enrollmentService;
private readonly ILogger<CoursesController> _logger;

public CoursesController(
    ICourseService courseService,
    IEnrollmentService enrollmentService,
    ILogger<CoursesController> logger)
{
    _courseService     = courseService;
    _enrollmentService = enrollmentService;
    _logger            = logger;
}

    /// <summary>Get all courses with pagination, search, sorting, select, and expand options.</summary>
    /// <param name="request">Pagination and filter parameters.</param>
    /// <response code="200">Returns a paged list of courses.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<object>>), StatusCodes.Status200OK)]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<object>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
    [FromQuery] PaginationRequest request
    )
    {
        var parameters = new QueryParameters
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Search = request.Search,
            Sort = request.Sort,
            Select = request.Select,
            Expand = request.Expand
        };
        int? semesterId = null;
        var expandedRels = parameters.GetExpandedRelations().ToList();
        var selectedFields = parameters.GetSelectedFields().ToList();
        var includeSubjects = expandedRels.Contains("subjects");  // ← detect early

        var paged = await _courseService.GetAllAsync(parameters, semesterId, includeSubjects); // ← pass flag

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
            $"Retrieved {response.TotalCount} course(s)."));
    }

    /// <summary>Get a single course by ID.</summary>
    /// <param name="id">Course ID.</param>
    /// <response code="200">Course found.</response>
    /// <response code="404">Course not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _courseService.GetByIdWithDetailsAsync(id);
        if (model is null)
            return NotFound(ApiResponse<object>.Fail($"Course with ID {id} was not found."));

        return Ok(ApiResponse<CourseResponse>.Ok(model.ToResponse()));
    }

    /// <summary>Get all enrollments for a specific course.</summary>
    /// <param name="id">Course ID.</param>
    /// <param name="request">Pagination and projection parameters.</param>
    /// <response code="200">Returns paged enrollments for the course.</response>
    /// <response code="404">Course not found.</response>
    [HttpGet("{id:int}/enrollments")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnrollments(
        int id,
        [FromQuery] PaginationRequest request)
    {
        // verify course exists
        var course = await _courseService.GetByIdAsync(id);
        if (course is null)
            return NotFound(ApiResponse<object>.Fail($"Course with ID {id} was not found."));

        var parameters = new QueryParameters
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Search = request.Search,
            Sort = request.Sort,
            Select = request.Select,
            Expand = request.Expand
        };

        var expandedRels = parameters.GetExpandedRelations().ToList();
        var selectedFields = parameters.GetSelectedFields().ToList();
        var includeStudent = expandedRels.Contains("student");

        // force courseId filter to the route id
        var paged = await _enrollmentService.GetAllAsync(
            parameters, studentId: null, courseId: id,
            includeStudent: includeStudent, includeCourse: false);

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
            $"Retrieved {response.TotalCount} enrollment(s) for course {id}."));
    }

    /// <summary>Create a new course.</summary>
    /// <param name="request">Course data.</param>
    /// <response code="201">Course created successfully.</response>
    /// <response code="400">Validation failed or semester not found.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation failed.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

        try
        {
            var model   = request.ToModel();
            var created = await _courseService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.CourseId },
                ApiResponse<CourseResponse>.Ok(created.ToResponse(), "Course created successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>Update an existing course.</summary>
    /// <param name="id">Course ID.</param>
    /// <param name="request">Updated data.</param>
    /// <response code="200">Course updated.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="404">Course not found.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation failed.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

        try
        {
            var model   = request.ToModel();
            var updated = await _courseService.UpdateAsync(id, model);

            if (updated is null)
                return NotFound(ApiResponse<object>.Fail($"Course with ID {id} was not found."));

            return Ok(ApiResponse<CourseResponse>.Ok(updated.ToResponse(), "Course updated successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>Delete a course by ID.</summary>
    /// <param name="id">Course ID.</param>
    /// <response code="200">Course deleted.</response>
    /// <response code="404">Course not found.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _courseService.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Course with ID {id} was not found."));

        return Ok(ApiResponse<object>.Ok(new { }, "Course deleted successfully."));
    }
}
