using Bhd.Infrastructure.Persistance.Contexts;
using Bhd.Infrastructure.Persistance.DataSeeders;
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
            logger.LogInformation("Migraciones aplicadas exitosamente");

            var usersCount = await context.Users.CountAsync();
            if (usersCount == 0)
            {
                logger.LogInformation("Ejecutando data seeder...");
                var seeder = services.GetRequiredService<DataSeeder>();
                await seeder.SeedAsync();
                logger.LogInformation("Data seeder ejecutado exitosamente");
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Ocurrió un error al aplicar las migraciones o el seeding");
            throw;
        }
    }
}