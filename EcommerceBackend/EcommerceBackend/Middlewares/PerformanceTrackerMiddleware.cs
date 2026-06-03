using System.Diagnostics;
namespace EcommerceBackend.Middlewares;
public class PerformanceTrackerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceTrackerMiddleware> _logger;

    public PerformanceTrackerMiddleware(RequestDelegate next, ILogger<PerformanceTrackerMiddleware> logger) {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) { 
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation($"[INCOMING] Request: {context.Request.Method} -> {context.Request.Path}");

        await _next(context);

        stopwatch.Stop();
        _logger.LogInformation($"[OUTGOING] Response --> Status Code: {context.Response.StatusCode} | Time Laga: {stopwatch.ElapsedMilliseconds}ms");
    }
}
