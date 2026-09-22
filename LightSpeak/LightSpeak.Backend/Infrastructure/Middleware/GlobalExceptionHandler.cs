namespace LightSpeak.Backend.Infrastructure.Middleware;

using System.Diagnostics;
using System.Net;
using FluentValidation;
using LightSpeak.Backend.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        var (statusCode, tag, message, metadata) = exception switch
        {
            ValidationException validation => (
                HttpStatusCode.BadRequest,
                ErrorTags.Validation.Failed,
                "One or more validation errors occurred.",
                validation.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(group => group.Key, group => group.First().ErrorMessage)),
            _ => (HttpStatusCode.InternalServerError,
                  ErrorTags.Server.InternalError,
                  "An unexpected error occurred.",
                  (Dictionary<string, string>?)null),
        };

        logger.LogError(exception, "Unhandled exception — Tag: {Tag}, TraceId: {TraceId}", tag, traceId);

        httpContext.Response.StatusCode = (int)statusCode;

        var detail = environment.IsDevelopment()
            ? $"{exception.GetType().Name}: {exception.Message}\n{exception.StackTrace}"
            : null;

        await httpContext.Response.WriteAsJsonAsync(
            new ApiErrorResponse(tag, message, traceId, metadata, detail),
            cancellationToken);

        return true;
    }
}
