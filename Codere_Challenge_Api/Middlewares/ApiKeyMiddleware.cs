namespace Codere_Challenge_Api.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var apiKey = context.Request.Headers["x-api-key"].FirstOrDefault();

            if (context.Request.Path.StartsWithSegments("/api/Shows/Filter", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            if (!string.IsNullOrEmpty(apiKey) && _configuration.GetSection("AllowedApiKey").Get<string>()!.Contains(apiKey))
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
            }
        }
    }
}
