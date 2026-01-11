using Bhd.Infrastructure.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Bhd.WebApi.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Aplicando migraciones de base de datos...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Migraciones aplicadas exitosamente.");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Ocurrió un error al aplicar las migraciones.");
            throw;
        }
    }
}