using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Bhd.Application.Interfaces;
using Bhd.Application.Services;

namespace Bhd.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDocumentService, DocumentService>();

        return services;
    }
}