namespace StudentPortal.Repositories.Common;

public class QueryParameters
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int Page { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
    }

    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;

    /// <summary>Comma-separated fields for $select. e.g. "courseId,courseName"</summary>
    /// <summary>Comma-separated scalar fields to return. e.g. "courseId,courseName,semesterName"</summary>
    public string? Select { get; set; }

    /// <summary>Comma-separated navigation properties to expand. e.g. "semester,subjects"</summary>
    public string? Expand { get; set; }

    public IEnumerable<string> GetSelectedFields() =>
        string.IsNullOrWhiteSpace(Select)
            ? Enumerable.Empty<string>()
            : Select.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => f.Trim().ToLowerInvariant());

    public IEnumerable<string> GetExpandedRelations() =>
        string.IsNullOrWhiteSpace(Expand)
            ? Enumerable.Empty<string>()
            : Expand.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => f.Trim().ToLowerInvariant());
}
