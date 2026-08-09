using Serilog.Context;

namespace Snmp.ApiGateway.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-ID";

        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var existingId) ? existingId.ToString() : Guid.NewGuid().ToString();

            context.Items[HeaderName] = correlationId;

            context.Request.Headers["X-Correlation-ID"] = correlationId;

            context.Response.Headers[HeaderName] = correlationId;

            using(LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}
