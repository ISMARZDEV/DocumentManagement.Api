using System.Net;
using System.Text.Json;
using Bhd.Application.Exceptions;

namespace Bhd.WebApi.Middleware;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        var message = exception.Message;

        switch (exception)
        {
            case UnauthorizedException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;

            case ForbiddenException:
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                break;

            case BadRequestException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case KeyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case AppException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            default:
                _logger.LogError(exception, "Error no controlado");
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                message = "Ha ocurrido un error interno en el servidor.";
                break;
        }

        var result = JsonSerializer.Serialize(new { mensaje = message });
        await response.WriteAsync(result);
    }
}
