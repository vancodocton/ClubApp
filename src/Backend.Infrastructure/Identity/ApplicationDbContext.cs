using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClubApp.Backend.Infrastructure.Identity
{
    public class AppIdentityDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public AppIdentityDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {
        }
    }
}