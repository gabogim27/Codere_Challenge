namespace Codere_Challenge_Api.Middlewares.Extensions
{
    public static class ApiKeyMiddlewareExtension
    {
        public static IApplicationBuilder UseApiKeyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}
