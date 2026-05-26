using Microsoft.AspNetCore.Mvc;
using StudentPortal.API.DTOs.Request;
using StudentPortal.API.DTOs.Response;
using StudentPortal.API.Extensions;
using StudentPortal.Repositories.Common;
using StudentPortal.Services.Interfaces;

namespace StudentPortal.API.Controllers;

/// <summary>CRUD operations for Semesters.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _service;
    private readonly ILogger<SemestersController> _logger;

    public SemestersController(ISemesterService service, ILogger<SemestersController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    /// <summary>Get all semesters with pagination, search, sorting, select, and expand options.</summary>
    /// <param name="request">Pagination and filter parameters.</param>
    /// <response code="200">Returns a paged list of semesters.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<object>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationRequest request)
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

        var expandedRels = parameters.GetExpandedRelations().ToList();
        var selectedFields = parameters.GetSelectedFields().ToList();
        var includeCourses = expandedRels.Contains("courses");

        var paged = await _service.GetAllAsync(parameters, includeCourses);

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
            $"Retrieved {response.TotalCount} semester(s)."));
    }

    /// <summary>Get a single semester by ID (includes its courses).</summary>
    /// <param name="id">Semester ID.</param>
    /// <response code="200">Semester found.</response>
    /// <response code="404">Semester not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _service.GetByIdWithCoursesAsync(id);
        if (model is null)
            return NotFound(ApiResponse<object>.Fail($"Semester with ID {id} was not found."));

        return Ok(ApiResponse<SemesterResponse>.Ok(model.ToResponse()));
    }

    /// <summary>Create a new semester.</summary>
    /// <param name="request">Semester data.</param>
    /// <response code="201">Semester created successfully.</response>
    /// <response code="400">Validation failed.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSemesterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation failed.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

        if (request.EndDate <= request.StartDate)
            return BadRequest(ApiResponse<object>.Fail("EndDate must be after StartDate."));

        var model   = request.ToModel();
        var created = await _service.CreateAsync(model);

        return CreatedAtAction(nameof(GetById), new { id = created.SemesterId },
            ApiResponse<SemesterResponse>.Ok(created.ToResponse(), "Semester created successfully."));
    }

    /// <summary>Update an existing semester.</summary>
    /// <param name="id">Semester ID.</param>
    /// <param name="request">Updated data.</param>
    /// <response code="200">Semester updated.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="404">Semester not found.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSemesterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation failed.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

        if (request.EndDate <= request.StartDate)
            return BadRequest(ApiResponse<object>.Fail("EndDate must be after StartDate."));

        var model   = request.ToModel();
        var updated = await _service.UpdateAsync(id, model);

        if (updated is null)
            return NotFound(ApiResponse<object>.Fail($"Semester with ID {id} was not found."));

        return Ok(ApiResponse<SemesterResponse>.Ok(updated.ToResponse(), "Semester updated successfully."));
    }

    /// <summary>Delete a semester by ID.</summary>
    /// <param name="id">Semester ID.</param>
    /// <response code="200">Semester deleted.</response>
    /// <response code="404">Semester not found.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Semester with ID {id} was not found."));

        return Ok(ApiResponse<object>.Ok(new { }, "Semester deleted successfully."));
    }
}
