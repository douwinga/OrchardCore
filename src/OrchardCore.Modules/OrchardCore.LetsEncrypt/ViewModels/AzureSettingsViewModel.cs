using System.Collections.Generic;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace OrchardCore.LetsEncrypt.ViewModels
{
    public class AzureSettingsViewModel
    {
        public ISet<string> HostNames { get; set; }
        public IReadOnlyDictionary<string, HostNameSslState> HostNameSslStates { get; set; }
        public IPagedCollection<IAppServiceCertificate> Certificates { get; internal set; }
    }
}
