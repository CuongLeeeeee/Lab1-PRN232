using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Interfaces;
using StudentPortal.Services.Interfaces;
using StudentPortal.Services.Mappings;
using StudentPortal.Services.Models;

namespace StudentPortal.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repo;
    private readonly ISemesterRepository _semesterRepo;

    public CourseService(ICourseRepository repo, ISemesterRepository semesterRepo)
    {
        _repo         = repo;
        _semesterRepo = semesterRepo;
    }

    public async Task<PagedResult<CourseModel>> GetAllAsync(QueryParameters parameters, int? semesterId = null, bool includeSubjects = false)
    {
        var paged = await _repo.SearchAsync(parameters, semesterId, includeSubjects);
        return new PagedResult<CourseModel>
        {
            Items = paged.Items.Select(e => e.ToModel()),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
    }

    public async Task<CourseModel?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity?.ToModel();
    }

    public async Task<CourseModel?> GetByIdWithDetailsAsync(int id)
    {
        var entity = await _repo.GetByIdWithDetailsAsync(id);
        return entity?.ToModel();
    }

    public async Task<CourseModel> CreateAsync(CourseModel model)
    {
        // Validate semester exists
        if (!await _semesterRepo.ExistsAsync(model.SemesterId))
            throw new InvalidOperationException($"Semester with ID {model.SemesterId} does not exist.");

        var entity  = model.ToEntity();
        var created = await _repo.CreateAsync(entity);
        return created.ToModel();
    }

    public async Task<CourseModel?> UpdateAsync(int id, CourseModel model)
    {
        if (!await _repo.ExistsAsync(id)) return null;

        if (!await _semesterRepo.ExistsAsync(model.SemesterId))
            throw new InvalidOperationException($"Semester with ID {model.SemesterId} does not exist.");

        model.CourseId = id;
        var entity  = model.ToEntity();
        var updated = await _repo.UpdateAsync(entity);
        return updated.ToModel();
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);
}
