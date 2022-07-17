using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

namespace ClubApp.Backend.Options
{
    internal static class IdentityServerConfig
    {
        public static string[] ReactClientOrigins = null!;

        public static ICollection<Client> Clients => new List<Client>()
        {
            ClientBuilder.SPA("ClubApp.Application")
            .WithRedirectUris("/authentication/login-callback", ReactClientOrigins)
            .WithLogoutRedirectUris("/authentication/logout-callback", ReactClientOrigins)
            .WithCorsOrigins(ReactClientOrigins)
            .AllowOfflineAccess()
            .Build(),
            ClientBuilder.SPA("ClubAppOidc")
            .WithRedirectUris("/.auth/login/oidc/callback", ReactClientOrigins)
            .WithLogoutRedirectUris("/authentication/logout-callback", ReactClientOrigins)
            .WithCorsOrigins(ReactClientOrigins)
            .WithClientSecret("secret".Sha256())
            .AllowOfflineAccess()
            .Build(),
        };
    }
}
