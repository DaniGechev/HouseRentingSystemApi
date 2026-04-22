using System.Diagnostics;

namespace HouseRentingSystemApi.Middlewares
{
    public class UseStopWatch
    {
        private readonly RequestDelegate next;
        private readonly ILogger<UseStopWatch> logger;

        public UseStopWatch(RequestDelegate next, ILogger<UseStopWatch> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await next(context);

            stopwatch.Stop();

            logger.LogInformation(
                "Request {method} {path} took {ms}ms - Status: {status}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                context.Response.StatusCode
            );
        }
    }

    public static class UseStopWatchExtensions
    {
        public static IApplicationBuilder UseStopwatch(this IApplicationBuilder app)
        {
            return app.UseMiddleware<UseStopWatch>();
        }
    }
}