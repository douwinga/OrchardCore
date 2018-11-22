using System.Threading.Tasks;
using Microsoft.Azure.Management.Dns;
using Microsoft.Azure.Management.WebSites;

namespace OrchardCore.LetsEncrypt.Services
{
    public interface IArmService
    {
        Task<WebSiteManagementClient> GetWebSiteManagementClient();
        Task<DnsManagementClient> GetDnsManagementClient();
    }
}
