using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Interfaces;
using StudentPortal.Services.Interfaces;
using StudentPortal.Services.Mappings;
using StudentPortal.Services.Models;

namespace StudentPortal.Services.Implementations;

public class SemesterService : ISemesterService
{
    private readonly ISemesterRepository _repo;

    public SemesterService(ISemesterRepository repo) => _repo = repo;

    public async Task<PagedResult<SemesterModel>> GetAllAsync(QueryParameters parameters, bool includeCourses = false)
    {
        var paged = await _repo.SearchAsync(parameters, includeCourses);
        return new PagedResult<SemesterModel>
        {
            Items = paged.Items.Select(e => e.ToModel()),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
    }

    public async Task<SemesterModel?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity?.ToModel();
    }

    public async Task<SemesterModel?> GetByIdWithCoursesAsync(int id)
    {
        var entity = await _repo.GetByIdWithCoursesAsync(id);
        return entity?.ToModel();
    }

    public async Task<SemesterModel> CreateAsync(SemesterModel model)
    {
        var entity  = model.ToEntity();
        var created = await _repo.CreateAsync(entity);
        return created.ToModel();
    }

    public async Task<SemesterModel?> UpdateAsync(int id, SemesterModel model)
    {
        if (!await _repo.ExistsAsync(id)) return null;
        model.SemesterId = id;
        var entity  = model.ToEntity();
        var updated = await _repo.UpdateAsync(entity);
        return updated.ToModel();
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);
}
