using Bhd.Application;
using Bhd.Infrastructure;
using Bhd.WebApi.Extensions;
using Hangfire;
using Hangfire.Dashboard;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddPresentationServices();
builder.Services.AddCorsConfiguration();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Apply database migrations
await app.ApplyMigrationsAsync();

// Configure middleware pipeline
app.UseApplicationMiddleware();

// Habilitar Hangfire Dashboard (solo en Development)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() }
    });
}

// Map endpoints
app.UseHealthCheckEndpoints();
app.MapControllers();

app.Run();


public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // dev, permitir acceso sin autenticación
        // prod, agregar validación JWT u otra autenticación
        return true;
    }
}