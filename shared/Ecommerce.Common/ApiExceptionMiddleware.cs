using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Common
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;

        public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var status = (int)HttpStatusCode.InternalServerError;
            string message = ApiMessages.RetrieveError;

            if (exception is ArgumentException || exception is InvalidOperationException)
            {
                status = (int)HttpStatusCode.BadRequest;
                message = string.IsNullOrWhiteSpace(exception.Message) ? ApiMessages.RetrieveError : exception.Message;
            }
            else if (exception is KeyNotFoundException || exception.GetType().Name == "NotFoundException")
            {
                status = (int)HttpStatusCode.NotFound;
                message = string.IsNullOrWhiteSpace(exception.Message) ? ApiMessages.IdNotFound : exception.Message;
            }
            else if (exception.GetType().Name == "ValidationException")
            {
                status = (int)HttpStatusCode.BadRequest;
                message = string.IsNullOrWhiteSpace(exception.Message) ? ApiMessages.RetrieveError : exception.Message;
            }
            else if (exception is UnauthorizedAccessException)
            {
                status = (int)HttpStatusCode.Unauthorized;
                message = string.IsNullOrWhiteSpace(exception.Message) ? ApiMessages.RetrieveError : exception.Message;
            }

            _logger.LogError(exception, ApiLogMessages.UnhandledException, exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = status;

            var apiResponse = ApiResponse.Fail(message);
            var json = JsonSerializer.Serialize(apiResponse);

            return context.Response.WriteAsync(json);
        }
    }
}
