using System.Diagnostics;

namespace PRN232.StuPortal.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _logFilePath;
    private static readonly object FileLock = new();

    public RequestLoggingMiddleware(RequestDelegate next, IWebHostEnvironment env)
    {
        _next = next;

        var logsDirectory = Path.Combine(env.ContentRootPath, "Logs");
        Directory.CreateDirectory(logsDirectory);
        _logFilePath = Path.Combine(logsDirectory, "request.log");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        var logLine =
            $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] " +
            $"{context.Request.Method} {context.Request.Path} " +
            $"-> {context.Response.StatusCode} ({stopwatch.ElapsedMilliseconds} ms)";

        Console.WriteLine(logLine);

        lock (FileLock)
        {
            File.AppendAllText(_logFilePath, logLine + Environment.NewLine);
        }
    }
}
