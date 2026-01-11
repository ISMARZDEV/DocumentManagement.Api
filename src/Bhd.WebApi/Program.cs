var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health Checks
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var rabbitMqHost = builder.Configuration["RabbitMq:Host"] ?? "localhost";
var rabbitMqUri = $"amqp://guest:guest@{rabbitMqHost}:5672";

builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: connectionString!,
        name: "sqlserver",
        tags: new[] { "db", "sql", "sqlserver" })
    .AddRabbitMQ(
        rabbitConnectionString: rabbitMqUri,
        name: "rabbitmq",
        tags: new[] { "messaging", "rabbitmq" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Deshabilitado en Docker

// Health Check Endpoint
app.MapHealthChecks("/health");

// Health Check endpoint documentado en Swagger
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

app.Run();
