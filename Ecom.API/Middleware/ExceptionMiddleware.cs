using Ecom.API.Helper;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.Json;

namespace Ecom.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IMemoryCache _memoryCache;
        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment hostEnvironment, IMemoryCache memoryCache)
        {
            this.next = next;
            this._hostEnvironment = hostEnvironment;
            this._memoryCache = memoryCache;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                ApplySecuirty(context);
                if (IsRequestAllowed(context) == false)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.ContentType = "application/json";
                    var response = new ApiException((int)HttpStatusCode.TooManyRequests,
                    "Too many requests. Please try again later.");
                    var json = JsonSerializer.Serialize(response);
                    await context.Response.WriteAsync(json);
                    return;
                }
                await this.next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var response = this._hostEnvironment.IsDevelopment() ?
                    new ApiException((int)HttpStatusCode.InternalServerError,
                    ex.Message, ex.StackTrace)
                    : new ApiException((int)HttpStatusCode.InternalServerError,
                    ex.Message);
                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);

            }
        }
        private bool IsRequestAllowed(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress.ToString();
            var cacheKey = $"Rate:{ip}";
            var DateNow = DateTime.UtcNow;
            var (timeStamp, count) = this._memoryCache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
                return (timeStamp: DateTime.UtcNow,count: 0);
            });
            if(DateNow - timeStamp < TimeSpan.FromSeconds(30))
            {
                if(count >= 20)
                {
                    return false; // Block the request
                }
                this._memoryCache.Set(cacheKey, (timeStamp, count + 1), TimeSpan.FromSeconds(30));
            }
            else
            {
                this._memoryCache.Set(cacheKey, (DateTime.UtcNow, 1));
            }
            return true; // Allow all requests for this example

        }

        private void ApplySecuirty(HttpContext context)
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            context.Response.Headers["X-Frame-Options"] = "DENY";
        }
    }
}
