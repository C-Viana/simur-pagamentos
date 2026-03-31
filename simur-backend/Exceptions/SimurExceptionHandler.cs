using Microsoft.IdentityModel.Tokens;
using simur_backend.Exceptions.CustomExceptions;
using System.Runtime.Serialization;
using System.Text.Json;

namespace simur_backend.Exceptions
{
    public class SimurExceptionHandler(RequestDelegate next, ILogger<SimurExceptionHandler> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<SimurExceptionHandler> _logger = logger;

        private static (int statusCode, string message) MapException(Exception ex)
        {
            return ex switch
            {
                ArgumentNullException => (400, ex.Message ?? "Invalid request"),
                JsonException => (400, ex.Message ?? "Invalid request"),
                BadHttpRequestException => (400, ex.Message ?? "Invalid request"),
                SecurityTokenException => (400, ex.Message ?? "Invalid request"),
                InvalidOperationException => (409, ex.Message ?? "Invalid operation"),
                KeyNotFoundException => (404, ex.Message ?? "Resource not found"),
                SerializationException => (500, ex.Message ?? "Failed to serialize/deserialize value"),
                PaymentCreationErrorException => (404, ex.Message ?? "No payment(s) found"),
                _ => (500, ex.Message ?? "Internal server error")
            };
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = MapException(exception);
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync(new
            {
                message,
                detail = exception.Message
            });
        }
    }
}
