using Snmp.Business.DTOs.Common;
using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;

namespace Snmp.WebAPI.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            catch(Exception ex)
            { 
                _logger.LogError(ex, "An unexpected error occurred in the system! Request: {Path}", context.Request.Path);
                await HandleExceptionAsync(context,ex);

            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            ErrorResult result = new ErrorResult
            {
                StatusCode = context.Response.StatusCode,
                Message = "A server error occurred during the process. Please try again later"
            };
            string json = JsonSerializer.Serialize(result);
            return context.Response.WriteAsync(json);
        }
    }
}
