using Microsoft.Extensions.Options;
using RepoGateway.Configuration;
using StackExchange.Redis;

namespace RepoGateway.Middlewares
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConnectionMultiplexer _redis;
        private readonly RateLimitConfig config;

        public RateLimitMiddleware(RequestDelegate next, IConnectionMultiplexer redis, IOptions<RateLimitConfig> options)
        {
            _next = next;
            _redis = redis;
            config = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/Repo"))
            {
                var db = _redis.GetDatabase();
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var userKey = $"rate:{ip}";
                var count = await db.StringIncrementAsync(userKey);

                if (count == 1)
                    await db.KeyExpireAsync(userKey, TimeSpan.FromMinutes(config.RateWindowsMinutes)); // rate-limit window

                if (count > config.MaxRequestsPerMinute) // max 10 requests per minute
                {
                    context.Response.StatusCode = 429;
                    await context.Response.WriteAsync("Rate limit exceeded");
                    return;
                }
            }

            await _next(context);
        }
    }
}
