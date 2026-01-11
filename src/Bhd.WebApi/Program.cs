using Bhd.Application;
using Bhd.Infrastructure;
using Bhd.WebApi.Extensions;

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

// Map endpoints
app.UseHealthCheckEndpoints();
app.MapControllers();

app.Run();