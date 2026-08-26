using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using RentalAttireBackend.Application.Common.Models;
using System.Runtime.CompilerServices;

namespace RentalAttireBackend.Application.Common.Extensions
{
    public static class ResultExtension
    {
        public static IActionResult ToActionResult<T>(
            this Result<T> result, 
            ControllerBase controller,
            IHttpContextAccessor httpContextAccessor)
        {
            if (result.IsSuccess)
                return controller.Ok(result);

            var statusCode = result.ErrorType switch
            {
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.BadRequest => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            var title = result.ErrorType switch
            {
                ErrorType.Conflict => "Conflict.",
                ErrorType.Unauthorized => "Unauthorized.",
                ErrorType.BadRequest => "Request Failed",
                ErrorType.NotFound => "Resource not found.",
                _ => "An unexpected error occured."
            };

            var instance = httpContextAccessor.HttpContext?.Request.Path;



            return controller.Problem(
                detail: result.ErrorMessage, 
                statusCode: statusCode, 
                title: title, 
                instance: instance);
        }
    }
}
