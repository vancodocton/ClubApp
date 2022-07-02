using Duende.IdentityServer;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

namespace ClubApp.Backend
{
    public static class ClientBuilderExtensions
    {
        public static ClientBuilder AllowOfflineAccess(this ClientBuilder builder)
        {
            var client = builder.Build();

            client.AllowOfflineAccess = true;
            client.AllowedScopes.Add(IdentityServerConstants.StandardScopes.OfflineAccess);

            return new ClientBuilder(client);
        }        
        
        public static ClientBuilder WithCorsOrigins(this ClientBuilder builder, params string[] origins)
        {
            var client = builder.Build();

            foreach (var origin in origins)
                client.AllowedCorsOrigins.Add(origin);

            return new ClientBuilder(client);
        }
    }
}