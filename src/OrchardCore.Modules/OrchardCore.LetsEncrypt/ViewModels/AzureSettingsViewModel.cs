using System.Collections.Generic;
using Microsoft.Azure.Management.WebSites.Models;

namespace OrchardCore.LetsEncrypt.ViewModels
{
    public class AzureSettingsViewModel
    {
        public IList<string> HostNames { get; set; }
        public IList<HostNameSslState> HostNameSslStates { get; set; }
        public IList<Certificate> Certificates { get; internal set; }
    }
}
