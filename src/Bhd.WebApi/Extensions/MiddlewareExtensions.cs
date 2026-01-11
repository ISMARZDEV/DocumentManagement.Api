using Bhd.WebApi.Middleware;

namespace Bhd.WebApi.Extensions;

public static class MiddlewareExtensions
{
    public static WebApplication UseApplicationMiddleware(this WebApplication app)
    {
        // Global Exception Handler
        app.UseMiddleware<GlobalExceptionHandler>();

        // Development specific
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // CORS
        app.UseCors("AllowAll");

        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication UseHealthCheckEndpoints(this WebApplication app)
    {
        // Basic Health Check
        app.MapHealthChecks("/health");

        // Detailed Health Check
        app.MapGet("/api/health", async (Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService healthCheckService) =>
        {
            var report = await healthCheckService.CheckHealthAsync();
            return Results.Ok(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description ?? "OK",
                    duration = e.Value.Duration.TotalMilliseconds
                })
            });
        })
        .WithName("GetHealth")
        .WithTags("Health")
        .WithOpenApi();

        return app;
    }
}