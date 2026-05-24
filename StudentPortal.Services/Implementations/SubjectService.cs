using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Interfaces;
using StudentPortal.Services.Interfaces;
using StudentPortal.Services.Mappings;
using StudentPortal.Services.Models;

namespace StudentPortal.Services.Implementations;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _repo;

    public SubjectService(ISubjectRepository repo) => _repo = repo;

    public async Task<PagedResult<SubjectModel>> GetAllAsync(QueryParameters parameters, bool includeCourses = false)
    {
        var paged = await _repo.SearchAsync(parameters, includeCourses);
        return new PagedResult<SubjectModel>
        {
            Items = paged.Items.Select(e => e.ToModel()),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
    }
    public async Task<SubjectModel?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity?.ToModel();
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        => await _repo.CodeExistsAsync(code, excludeId);

    public async Task<SubjectModel> CreateAsync(SubjectModel model)
    {
        if (await _repo.CodeExistsAsync(model.SubjectCode))
            throw new InvalidOperationException($"Subject code '{model.SubjectCode}' already exists.");

        var entity  = model.ToEntity();
        var created = await _repo.CreateAsync(entity);
        return created.ToModel();
    }

    public async Task<SubjectModel?> UpdateAsync(int id, SubjectModel model)
    {
        if (!await _repo.ExistsAsync(id)) return null;

        if (await _repo.CodeExistsAsync(model.SubjectCode, excludeId: id))
            throw new InvalidOperationException($"Subject code '{model.SubjectCode}' is already used by another subject.");

        model.SubjectId = id;
        var entity  = model.ToEntity();
        var updated = await _repo.UpdateAsync(entity);
        return updated.ToModel();
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);
}
