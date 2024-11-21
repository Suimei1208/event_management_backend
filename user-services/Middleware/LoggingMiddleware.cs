using System.Diagnostics;

namespace user_services.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var method = context.Request.Method;

            try
            {
                await _next(context);

                stopwatch.Stop();
                _logger.LogInformation(
                    "Method: {Method}, Path: {Path}, StatusCode: {StatusCode}, Duration: {Duration}ms",
                    method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds
                );
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "Method: {Method}, Path: {Path}, Duration: {Duration}ms, Exception: {Exception}",
                    method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds,
                    ex.Message
                );
                throw;
            }
        }
    }
}
