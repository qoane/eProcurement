using System.Diagnostics;
using System.Security.Claims;
using Lca.EProcurement.Application;
using Lca.EProcurement.Domain;
using Lca.EProcurement.EntityFrameworkCore;

namespace Lca.EProcurement.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-ID";
    public async Task InvokeAsync(HttpContext context)
    {
        var id = context.Request.Headers.TryGetValue(HeaderName, out var values) && !string.IsNullOrWhiteSpace(values) ? values.ToString() : Guid.NewGuid().ToString("N");
        context.Items[HeaderName] = id;
        context.Response.OnStarting(() => { context.Response.Headers[HeaderName] = id; return Task.CompletedTask; });
        await next(context);
    }
}
public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IConfiguration config)
{
    public async Task InvokeAsync(HttpContext context, IOperationsApplicationService ops)
    {
        var sw=Stopwatch.StartNew(); await next(context); sw.Stop();
        var correlation=(string?)context.Items[CorrelationIdMiddleware.HeaderName] ?? "unknown";
        var userId=context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.User.Identity?.Name;
        logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {DurationMs} ms CorrelationId={CorrelationId} UserId={UserId} UserEmail={UserEmail} ClientIp={ClientIp} UserAgent={UserAgent}", context.Request.Method, context.Request.Path.Value, context.Response.StatusCode, sw.ElapsedMilliseconds, correlation, userId, context.User.FindFirstValue(ClaimTypes.Email), context.Connection.RemoteIpAddress?.ToString(), context.Request.Headers["User-Agent"].ToString());
        if (config.GetValue("Operations:EnablePerformanceSampling", true))
        {
            var slow=sw.ElapsedMilliseconds>=config.GetValue("Operations:SlowRequestThresholdMs",1000); var failed=context.Response.StatusCode>=400; var pct=config.GetValue("Operations:PerformanceSamplingPercentage",10);
            if (slow || failed || Random.Shared.Next(100)<pct) await ops.RecordPerformanceAsync(new ApiPerformanceSample(correlation, context.Request.Path.Value ?? "", context.Request.Method, context.Response.StatusCode, sw.ElapsedMilliseconds, userId, DateTimeOffset.UtcNow));
        }
    }
}
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env, IConfiguration config, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, EProcurementDbContext db)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            var correlation=(string?)context.Items[CorrelationIdMiddleware.HeaderName] ?? Guid.NewGuid().ToString("N");
            logger.LogError(ex,"Unhandled exception CorrelationId={CorrelationId}", correlation);
            db.AuditEvents.Add(new AuditEvent("Unhandled exception","HTTP",Guid.Empty,"Exception",context.User.Identity?.Name??"anonymous",correlation,DateTimeOffset.UtcNow)); await db.SaveChangesAsync();
            var details=env.IsDevelopment() && config.GetValue("Operations:ShowDetailedErrorsInDevelopment", false) ? ex.ToString() : null;
            context.Response.StatusCode=ex switch { UnauthorizedAccessException => StatusCodes.Status403Forbidden, InvalidOperationException => StatusCodes.Status422UnprocessableEntity, ArgumentException => StatusCodes.Status400BadRequest, KeyNotFoundException => StatusCodes.Status404NotFound, _ => StatusCodes.Status500InternalServerError };
            context.Response.ContentType="application/json";
            await context.Response.WriteAsJsonAsync(new { correlationId=correlation, statusCode=context.Response.StatusCode, errorCode=context.Response.StatusCode==500?"UnhandledError":"RequestError", message=context.Response.StatusCode==500?"An unexpected error occurred.":ex.Message, details, timestamp=DateTimeOffset.UtcNow });
        }
    }
}
