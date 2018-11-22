using System;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Dns;
using Microsoft.Azure.Management.WebSites;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using OrchardCore.LetsEncrypt.Settings;

namespace OrchardCore.LetsEncrypt.Services
{
    public class ArmService : IArmService
    {
        private readonly LetsEncryptAzureAuthSettings _options;

        public ArmService(IOptions<LetsEncryptAzureAuthSettings> options)
        {
            _options = options.Value;
        }

        public async Task<WebSiteManagementClient> GetWebSiteManagementClient()
        {
            var token = await GetToken();
            var tokenCredentials = new TokenCredentials(token.AccessToken);

            var websiteClient = new WebSiteManagementClient(new Uri("https://management.azure.com"), tokenCredentials)
            {
                SubscriptionId = _options.SubscriptionId
            };
            return websiteClient;
        }

        public async Task<DnsManagementClient> GetDnsManagementClient()
        {
            var token = await GetToken();
            var tokenCredentials = new TokenCredentials(token.AccessToken);

            var dnsClient = new DnsManagementClient(new Uri("https://management.azure.com"), tokenCredentials)
            {
                SubscriptionId = _options.SubscriptionId
            };
            return dnsClient;
        }

        private async Task<AuthenticationResult> GetToken()
        {
            var authContext = new AuthenticationContext($"https://login.windows.net/{_options.Tenant}");

            return await authContext.AcquireTokenAsync("https://management.core.windows.net/", new ClientCredential(_options.ClientId, _options.ClientSecret));
        }
    }
}
