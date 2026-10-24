using System.Diagnostics;
using HomeMate.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HomeMate.Shared.Web;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        var (statusCode, title, detail) = MapException(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception: {Message}. TraceId: {TraceId}", exception.Message, traceId);
        }
        else
        {
            _logger.LogWarning("Handled exception: {Message}. Status: {StatusCode}. TraceId: {TraceId}", exception.Message, statusCode, traceId);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = traceId;

        if (exception is ValidationException validationException && validationException.Errors.Count > 0)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        httpContext.Response.StatusCode = statusCode;

        var handled = await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });

        if (!handled)
        {
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        }

        return true;
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "Validation Failed",
                validationException.Message
            ),
            BadRequestException badRequestException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                badRequestException.Message
            ),
            UnauthorizedException unauthorizedException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                unauthorizedException.Message
            ),
            ForbiddenException forbiddenException => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                forbiddenException.Message
            ),
            NotFoundException notFoundException => (
                StatusCodes.Status404NotFound,
                "Not Found",
                notFoundException.Message
            ),
            ConflictException conflictException => (
                StatusCodes.Status409Conflict,
                "Conflict",
                conflictException.Message
            ),
            AppException appException => (
                (int)appException.StatusCode,
                "Application Error",
                appException.Message
            ),
            ArgumentException argumentException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                argumentException.Message
            ),
            InvalidOperationException invalidOperationException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                invalidOperationException.Message
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected server error occurred.",
                "Please contact support if the issue persists."
            )
        };
    }
}
