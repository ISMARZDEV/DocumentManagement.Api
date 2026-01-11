using System.Text;
using Bhd.Application.Interfaces;
using Bhd.Domain.Interfaces;
using Bhd.Infrastructure.Identity;
using Bhd.Infrastructure.Persistance.Contexts;
using Bhd.Infrastructure.Persistance.Repositories;
using Bhd.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Bhd.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddJwtAuthentication(configuration);
        services.AddHealthChecksConfiguration(configuration);

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtGenerator, JwtGenerator>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }

    private static IServiceCollection AddHealthChecksConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqHost = configuration["RabbitMq:Host"] ?? "localhost";
        var rabbitMqUri = $"amqp://guest:guest@{rabbitMqHost}:5672";

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>(
                name: "sqlserver",
                tags: new[] { "db", "sql", "sqlserver" })
            .AddRabbitMQ(
                rabbitConnectionString: rabbitMqUri,
                name: "rabbitmq",
                tags: new[] { "messaging", "rabbitmq" });

        return services;
    }
}