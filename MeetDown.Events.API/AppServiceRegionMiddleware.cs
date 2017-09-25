using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace MeetDown.Events.API
{
    public class AppServiceRegionMiddleware
    {
        private readonly RequestDelegate _next;

        public AppServiceRegionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(state => {

                var httpContext = (HttpContext)state;
                httpContext.Response.Headers.Add("X-RegionName", Environment.GetEnvironmentVariable("REGION_NAME") ?? "Local");

                return Task.FromResult(0);
            }, context);

            await _next(context);
        }
    }

    public static class AppServiceRegionMiddlewareExtensions
    {
        public static IApplicationBuilder UseAppServiceRegionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AppServiceRegionMiddleware>();
        }
    }
}
