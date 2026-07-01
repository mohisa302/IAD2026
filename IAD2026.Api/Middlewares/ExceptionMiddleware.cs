using IAD2026.Application.Exceptions;
using IAD2026.Shared;

namespace IAD2026.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        ApiResponse<object> response;
        int statusCode;

        switch (exception)
        {
            case ExternalApiException ex:
                statusCode = (int)ex.StatusCode;
                response = ApiResponse<object>.Error(ex.ErrorCode, ex.Message, statusCode);
                context.Response.Headers.Append("X-Error-Code", ex.ErrorCode);
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                response = ApiResponse<object>.Error(ErrorCodes.InternalServerError, "An unexpected error occurred", statusCode);
                context.Response.Headers.Append("X-Error-Code", ErrorCodes.InternalServerError);
                break;
        }

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}