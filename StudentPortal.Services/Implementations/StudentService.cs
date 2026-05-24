using StudentPortal.Repositories.Common;
using StudentPortal.Repositories.Interfaces;
using StudentPortal.Services.Interfaces;
using StudentPortal.Services.Mappings;
using StudentPortal.Services.Models;

namespace StudentPortal.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repo;

    public StudentService(IStudentRepository repo) => _repo = repo;

    public async Task<PagedResult<StudentModel>> GetAllAsync(QueryParameters parameters, bool includeCourses = false)
    {
        var paged = await _repo.SearchAsync(parameters, includeCourses);
        return new PagedResult<StudentModel>
        {
            Items = paged.Items.Select(e => e.ToModel()),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
    }

    public async Task<StudentModel?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity?.ToModel();
    }

    public async Task<StudentModel?> GetByIdWithEnrollmentsAsync(int id)
    {
        var entity = await _repo.GetByIdWithEnrollmentsAsync(id);
        return entity?.ToModel();
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        => await _repo.EmailExistsAsync(email, excludeId);

    public async Task<StudentModel> CreateAsync(StudentModel model)
    {
        if (await _repo.EmailExistsAsync(model.Email))
            throw new InvalidOperationException($"Email '{model.Email}' is already registered.");

        var entity  = model.ToEntity();
        var created = await _repo.CreateAsync(entity);
        return created.ToModel();
    }

    public async Task<StudentModel?> UpdateAsync(int id, StudentModel model)
    {
        if (!await _repo.ExistsAsync(id)) return null;

        if (await _repo.EmailExistsAsync(model.Email, excludeId: id))
            throw new InvalidOperationException($"Email '{model.Email}' is already used by another student.");

        model.StudentId = id;
        var entity  = model.ToEntity();
        var updated = await _repo.UpdateAsync(entity);
        return updated.ToModel();
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);
}
