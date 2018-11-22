using Microsoft.Azure.Management.WebSites;
using Microsoft.Azure.Management.WebSites.Models;

namespace OrchardCore.LetsEncrypt.Extrensions
{
    public static class SiteSlotExtensions
    {
        public static Site GetSiteOrSlot(this IWebAppsOperations sites, string resourceGroupName, string webAppName,
            string siteSlotName)
        {
            if (string.IsNullOrEmpty(siteSlotName))
            {
                return sites.Get(resourceGroupName, webAppName);
            }
            else
            {
                return sites.GetSlot(resourceGroupName, webAppName, siteSlotName);
            }
        }
    }
}
