using API.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace API.Filters
{
    /// <summary>
    /// Global exception filter ?? handle t?t c? exceptions
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Unhandled exception occurred");

            var response = ApiResponse<object>.ErrorResponse(
                "?ã x?y ra l?i không mong mu?n. Vui lòng th? l?i sau.",
           new List<string> { context.Exception.Message }
                   );

            context.Result = new ObjectResult(response)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            context.ExceptionHandled = true;
        }
    }

    /// <summary>
    /// Validate ModelState t? ??ng
    /// </summary>
    public class ValidateModelStateFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
  .Where(x => x.Value?.Errors.Count > 0)
       .SelectMany(x => x.Value!.Errors)
          .Select(x => x.ErrorMessage)
        .ToList();

                var response = ApiResponse<object>.ErrorResponse(
                         "D? li?u không h?p l?",
                   errors
                );

                context.Result = new BadRequestObjectResult(response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No implementation needed
        }
    }
}
