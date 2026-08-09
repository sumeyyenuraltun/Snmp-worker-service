using System.Diagnostics;
using System.Security.Claims;
using Serilog;
namespace Snmp.ApiGateway.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var user = context.User.Identity?.IsAuthenticated == true
                    ? context.User.FindFirst(ClaimTypes.Name)?.Value
                    : "Anonymous";

                var correlationId = context.Items["X-Correlation-ID"]?.ToString();

                Log.Information(
                    "CorrelationId:{CorrelationId} | HTTP {Method} {Path} responded {StatusCode} in {Elapsed:0.0000} ms | User:{User} | IP:{IP}",
                    correlationId,
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds,
                    user,
                    context.Connection.RemoteIpAddress?.ToString());
            }
        }
    }
}
