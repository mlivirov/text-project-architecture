using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StupidChat.Application.Common.Exceptions;
using ForbiddenException = StupidChat.Application.Common.Exceptions.AccessViolationException;
using ApplicationException = StupidChat.Application.Common.Exceptions.ApplicationException;

namespace StupidChat.WebApi;

public class ApplicationExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApplicationExceptionFilter> _logger;

    public ApplicationExceptionFilter(ILogger<ApplicationExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        _logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

        var response = exception switch
        {
            ValidationException validationEx => new BadRequestObjectResult(new
            {
                error = "Validation failed",
                details = validationEx.Message
            }),
            NotFoundException notFoundEx => new NotFoundObjectResult(new
            {
                error = "Resource not found",
                details = notFoundEx.Message
            }),
            ForbiddenException accessEx => new UnauthorizedObjectResult(new
            {
                error = "Access denied",
                details = accessEx.Message
            }),
            ApplicationException appEx => new BadRequestObjectResult(new
            {
                error = "Application error",
                details = appEx.Message
            }),
            _ => new ObjectResult(new
            {
                error = "An unexpected error occurred",
                details = "Please try again later"
            })
            {
                StatusCode = 500
            }
        };

        context.Result = response;
        context.ExceptionHandled = true;
    }
}