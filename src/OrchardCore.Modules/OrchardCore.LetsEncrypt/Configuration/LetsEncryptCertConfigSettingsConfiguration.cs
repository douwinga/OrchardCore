using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrchardCore.Entities;
using OrchardCore.LetsEncrypt.Settings;
using OrchardCore.Settings;

namespace OrchardCore.LetsEncrypt.Configuration
{
    public class LetsEncryptCertConfigSettingsConfiguration : IConfigureOptions<LetsEncryptCertConfigSettings>
    {
        private readonly ISiteService _site;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<LetsEncryptCertConfigSettingsConfiguration> _logger;

        public LetsEncryptCertConfigSettingsConfiguration(
            ISiteService site,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<LetsEncryptCertConfigSettingsConfiguration> logger)
        {
            _site = site;
            _dataProtectionProvider = dataProtectionProvider;
            _logger = logger;
        }

        public void Configure(LetsEncryptCertConfigSettings options)
        {
            var settings = _site.GetSiteSettingsAsync()
                .GetAwaiter().GetResult()
                .As<LetsEncryptCertConfigSettings>();

            options.Country = settings.Country;
            options.StateOrProvince = settings.StateOrProvince;
            options.Locality = settings.Locality;
            options.Organization = settings.Organization;
            options.OrganizationUnit = settings.OrganizationUnit;

            // Decrypt the PFX password
            if (!String.IsNullOrWhiteSpace(settings.PfxPassword))
            {
                try
                {
                    var protector = _dataProtectionProvider.CreateProtector(nameof(LetsEncryptCertConfigSettingsConfiguration));
                    options.PfxPassword = protector.Unprotect(settings.PfxPassword);
                }
                catch
                {
                    _logger.LogError("The PFX secret could not be decrypted. It may have been encrypted using a different key.");
                }
            }
        }
    }
}
