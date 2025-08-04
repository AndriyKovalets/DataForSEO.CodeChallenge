using System.Net;
using System.Text.Json;
using Dispatcher.Domain.Exceptions;

namespace Dispatcher.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (Exception exception)
        {
            switch (exception)
            {
                case HttpException ex:
                    await HandleHttpException(context, ex);
                    break;
                default:
                    await HandleGenericException(context, exception);
                    break;
            }
        }
    }

    private async Task HandleGenericException(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "Unknown error has occured");
        await CreateExceptionAsync(context);
    }

    private async Task HandleHttpException(HttpContext context, HttpException ex)
    {
        _logger.LogError(ex, ex.Message, ex.StatusCode);
        await CreateExceptionAsync(context, ex.StatusCode, new { error = ex.Message });
    }

    private async Task CreateExceptionAsync(
        HttpContext context,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
        object? errorBody = null)
    {
        errorBody ??= new { error = "Unknown error has occured" };
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorBody));
    }
}