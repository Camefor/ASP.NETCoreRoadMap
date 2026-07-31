using IdentityServer4.Models;
using System.Collections.Generic;

namespace ISExample.Settings
{
    public class IdentityServerSettings
    {

        public IReadOnlyCollection<ApiScope> ApiScopes { get; set; }
        public IReadOnlyCollection<ApiResource> ApiResources { get; set; }
        public IReadOnlyCollection<Client> Clients { get; set; }
        public IReadOnlyCollection<IdentityResource> IdentityResources => new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

    }
}
