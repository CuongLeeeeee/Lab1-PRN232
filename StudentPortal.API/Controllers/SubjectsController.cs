using Microsoft.AspNetCore.Mvc;
using StudentPortal.API.DTOs.Request;
using StudentPortal.API.DTOs.Response;
using StudentPortal.API.Extensions;
using StudentPortal.Repositories.Common;
using StudentPortal.Services.Interfaces;

namespace StudentPortal.API.Controllers;

/// <summary>CRUD operations for Subjects.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _service;
    private readonly ILogger<SubjectsController> _logger;

    public SubjectsController(ISubjectService service, ILogger<SubjectsController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    /// <summary>Get all subjects with pagination, search, sorting, select, and expand options.</summary>
    /// <param name="request">Pagination and filter parameters.</param>
    /// <response code="200">Returns a paged list of subjects.</response>
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
            $"Retrieved {response.TotalCount} subject(s)."));
    }

    /// <summary>Get a single subject by ID.</summary>
    /// <param name="id">Subject ID.</param>
    /// <response code="200">Subject found.</response>
    /// <response code="404">Subject not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _service.GetByIdAsync(id);
        if (model is null)
            return NotFound(ApiResponse<object>.Fail($"Subject with ID {id} was not found."));

        return Ok(ApiResponse<SubjectResponse>.Ok(model.ToResponse()));
    }

    /// <summary>Create a new subject.</summary>
    /// <param name="request">Subject data.</param>
    /// <response code="201">Subject created successfully.</response>
    /// <response code="400">Validation failed or subject code already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSubjectRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation failed.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

        try
        {
            var model   = request.ToModel();
            var created = await _service.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.SubjectId },
                ApiResponse<SubjectResponse>.Ok(created.ToResponse(), "Subject created successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>Update an existing subject.</summary>
    /// <param name="id">Subject ID.</param>
    /// <param name="request">Updated data.</param>
    /// <response code="200">Subject updated.</response>
    /// <response code="400">Validation failed or code conflict.</response>
    /// <response code="404">Subject not found.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSubjectRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation failed.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

        try
        {
            var model   = request.ToModel();
            var updated = await _service.UpdateAsync(id, model);

            if (updated is null)
                return NotFound(ApiResponse<object>.Fail($"Subject with ID {id} was not found."));

            return Ok(ApiResponse<SubjectResponse>.Ok(updated.ToResponse(), "Subject updated successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>Delete a subject by ID.</summary>
    /// <param name="id">Subject ID.</param>
    /// <response code="200">Subject deleted.</response>
    /// <response code="404">Subject not found.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Subject with ID {id} was not found."));

        return Ok(ApiResponse<object>.Ok(new { }, "Subject deleted successfully."));
    }
}
