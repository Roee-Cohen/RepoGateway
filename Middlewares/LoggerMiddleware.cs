using System.Diagnostics;

namespace RepoGateway.Middlewares
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine($"\r\nRequest: {context.Request.Method} {context.Request.Path}");
            Stopwatch stopwatch = Stopwatch.StartNew();
            await _next(context); // Call the next middleware
            stopwatch.Stop();
            // Code after next middleware (after controller executes)
            Console.WriteLine($"Response: {context.Request.Method} {context.Request.Path} finished with code {context.Response.StatusCode}, {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
