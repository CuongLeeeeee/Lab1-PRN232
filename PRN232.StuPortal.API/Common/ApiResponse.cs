namespace PRN232.StuPortal.API.Common
{
    public class ApiResponse<T>
    {
        
            public bool Success { get; set; }

            public string Message { get; set; } = "Request processed successfully";

            public T? Data { get; set; }

            public object? Errors { get; set; }

            public object? Pagination { get; set; }
        
    }
}
