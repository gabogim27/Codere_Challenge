using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Codere_Challenge_Api.Authorization
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly string ApiKeyHeaderName = "x-api-key";

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IConfiguration configuration) : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(ApiKeyHeaderName))
            {
                return Task.FromResult(AuthenticateResult.Fail("Header was not found"));
            }

            string token = Request.Headers[ApiKeyHeaderName].ToString();


            if (string.IsNullOrEmpty(token) || !IsApiKeyValid(token))
            {
                return Task.FromResult(AuthenticateResult.Fail("Token is invalid"));
            }
            else
            {
                var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, "Gabriel Gimenez"),
                    new Claim(ClaimTypes.Email, "gabogim27@gmail.com"),
                };

                var claimsIdentity = new ClaimsIdentity(claims, nameof(ApiKeyAuthenticationHandler));
                var authTicket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(authTicket));
            }
        }

        private bool IsApiKeyValid(string apiKey)
        {
            var allowedApiKey = _configuration.GetSection("AllowedApiKey").Get<string>();
            return !string.IsNullOrWhiteSpace(allowedApiKey) && allowedApiKey.Contains(apiKey);
        }
    }
}
