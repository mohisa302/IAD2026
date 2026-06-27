using IAD2026.Shared;
using Serilog;
using System.Net;

namespace IAD2026.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = ApiResponse<object>.ErrorResponse(
            errorCode: "INTERNAL_SERVER_ERROR",
            message: "An unexpected error occurred. Please try again later.",
            statusCode: 500
        );

        context.Response.Headers.Append("X-Error-Code", "INTERNAL_SERVER_ERROR");

        await context.Response.WriteAsJsonAsync(response);
    }
}