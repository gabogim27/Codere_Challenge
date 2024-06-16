using Microsoft.AspNetCore.Authentication;

namespace Codere_Challenge_Api.Authorization
{
    public class ApiKeyAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public string? ApiKey { get; set; }
    }
}
