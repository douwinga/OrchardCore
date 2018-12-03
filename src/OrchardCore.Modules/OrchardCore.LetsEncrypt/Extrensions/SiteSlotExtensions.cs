using System;
using Microsoft.Azure.Management.WebSites;
using Microsoft.Azure.Management.WebSites.Models;
using OrchardCore.LetsEncrypt.Settings;

namespace OrchardCore.LetsEncrypt.Extensions
{
    public static class SiteSlotExtensions
    {
        public static Site GetSiteOrSlot(this IWebAppsOperations sites, LetsEncryptAzureAuthSettings azureAuthSettings)
        {
            if (azureAuthSettings == null)
            {
                throw new ArgumentNullException(nameof(azureAuthSettings));
            }

            if (string.IsNullOrEmpty(azureAuthSettings.SiteSlotName))
            {
                return sites.Get(azureAuthSettings.ResourceGroupName, azureAuthSettings.WebAppName);
            }
            else
            {
                return sites.GetSlot(azureAuthSettings.ResourceGroupName, azureAuthSettings.WebAppName, azureAuthSettings.SiteSlotName);
            }
        }
    }
}
