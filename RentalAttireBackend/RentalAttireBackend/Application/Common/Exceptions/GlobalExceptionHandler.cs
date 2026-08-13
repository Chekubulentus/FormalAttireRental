using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.CodeDom;
using FluentValidation;

namespace RentalAttireBackend.Application.Common.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        public GlobalExceptionHandler(
            IProblemDetailsService problemDetailsService
            )
        {
            _problemDetailsService = problemDetailsService;
        }
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
            var statusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            httpContext.Response.StatusCode = statusCode;

            ProblemDetails problemDetails;

            if(exception is ValidationException validationException)
            {
                var errors = validationException.Errors
                    .GroupBy(err => err.PropertyName)
                    .ToDictionary(
                    g => g.Key,
                    g => g.Select(g => g.ErrorMessage).ToArray()
                    );

                problemDetails = new ValidationProblemDetails(errors)
                {
                    Status = statusCode,
                    Title = "Validation Failed.",
                    Detail = "One or more validation errors occured.",
                    Instance = httpContext.Request.Path
                };
            }else
            {
                problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = "An error occured.",
                    Detail = exception.Message,
                    Instance = httpContext.Request.Path
                };
            }

            return await _problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    Exception = exception,
                    ProblemDetails = problemDetails
                }
                );
        }
    }
}
