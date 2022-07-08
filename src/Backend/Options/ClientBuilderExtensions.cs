using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

namespace ClubApp.Backend.Options
{
    internal static class ClientBuilderExtensions
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

        public static ClientBuilder WithClientSecret(this ClientBuilder builder, string hashedSecret)
        {
            var client = builder.Build();

            client.RequireClientSecret = true;
            client.ClientSecrets.Add(new Secret(hashedSecret));

            return new ClientBuilder(client);
        }


        public static ClientBuilder WithRedirectUris(this ClientBuilder builder, string uri, params string[] origins)
        {
            foreach (var origin in origins)
            {
                builder.WithRedirectUri($"{origin}{uri}");
            }
            return builder;
        }
        public static ClientBuilder WithLogoutRedirectUris(this ClientBuilder builder, string uri, params string[] origins)
        {
            foreach (var origin in origins)
            {
                builder.WithLogoutRedirectUri($"{origin}{uri}");
            }
            return builder;
        }
    }
}