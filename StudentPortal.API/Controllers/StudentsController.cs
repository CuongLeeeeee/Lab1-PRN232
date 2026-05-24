using Microsoft.AspNetCore.Mvc;
using StudentPortal.API.DTOs.Request;
using StudentPortal.API.DTOs.Response;
using StudentPortal.API.Extensions;
using StudentPortal.Repositories.Common;
using StudentPortal.Services.Interfaces;

namespace StudentPortal.API.Controllers;

/// <summary>CRUD operations for Students.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentService service, ILogger<StudentsController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    /// <summary>Get all students with pagination, search, sorting, select, and expand options.</summary>
    /// <param name="request">Pagination and filter parameters.</param>
    /// <response code="200">Returns a paged list of students.</response>
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
            $"Retrieved {response.TotalCount} student(s)."));
    }

    /// <summary>Get a single student by ID (includes enrolled courses).</summary>
    /// <param name="id">Student ID.</param>
    /// <response code="200">Student found.</response>
    /// <response code="404">Student not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _service.GetByIdWithEnrollmentsAsync(id);
        if (model is null)
            return NotFound(ApiResponse<object>.Fail($"Student with ID {id} was not found."));

        return Ok(ApiResponse<StudentResponse>.Ok(model.ToResponse()));
    }

    /// <summary>Create a new student.</summary>
    /// <param name="request">Student data.</param>
    /// <response code="201">Student created successfully.</response>
    /// <response code="400">Validation failed or email already registered.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation failed.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

        try
        {
            var model   = request.ToModel();
            var created = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.StudentId },
                ApiResponse<StudentResponse>.Ok(created.ToResponse(), "Student created successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>Update an existing student.</summary>
    /// <param name="id">Student ID.</param>
    /// <param name="request">Updated data.</param>
    /// <response code="200">Student updated.</response>
    /// <response code="400">Validation failed or email conflict.</response>
    /// <response code="404">Student not found.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation failed.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

        try
        {
            var model   = request.ToModel();
            var updated = await _service.UpdateAsync(id, model);

            if (updated is null)
                return NotFound(ApiResponse<object>.Fail($"Student with ID {id} was not found."));

            return Ok(ApiResponse<StudentResponse>.Ok(updated.ToResponse(), "Student updated successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>Delete a student by ID.</summary>
    /// <param name="id">Student ID.</param>
    /// <response code="200">Student deleted.</response>
    /// <response code="404">Student not found.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Student with ID {id} was not found."));

        return Ok(ApiResponse<object>.Ok(new { }, "Student deleted successfully."));
    }
}
