using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Certes;
using Certes.Acme;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell;

namespace OrchardCore.LetsEncrypt.Services
{
    public class LetsEncryptService : ILetsEncryptService
    {
        private readonly IOptions<ShellOptions> _shellOptions;
        private readonly ShellSettings _shellSettings;

        public LetsEncryptService(
            IOptions<ShellOptions> shellOptions,
            ShellSettings shellSettings
            )
        {
            _shellOptions = shellOptions;
            _shellSettings = shellSettings;
        }

        public async Task RequestHttpChallengeCertificate(string registrationEmail, string[] hostnames, bool useStaging)
        {
            var acmeContext = await GetOrCreateAcmeContext(registrationEmail, useStaging);
        }

        private async Task<AcmeContext> GetOrCreateAcmeContext(string registrationEmail, bool useStaging)
        {
            AcmeContext acmeContext;

            var letsEncryptUri = useStaging ? WellKnownServers.LetsEncryptStagingV2 : WellKnownServers.LetsEncryptV2;

            var pemKeyFilename = PathExtensions.Combine(
                _shellOptions.Value.ShellsApplicationDataPath,
                _shellOptions.Value.ShellsContainerName,
                _shellSettings.Name,
                "Lets-Encrypt",
                $"{registrationEmail}-{letsEncryptUri.Host}.pem");

            if (!File.Exists(pemKeyFilename))
            {
                acmeContext = new AcmeContext(letsEncryptUri);
                var account = await acmeContext.NewAccount(registrationEmail, true);

                var pemKey = acmeContext.AccountKey.ToPem();

                Directory.CreateDirectory(Path.GetDirectoryName(pemKeyFilename));
                File.WriteAllText(pemKeyFilename, pemKey);

                acmeContext = new AcmeContext(letsEncryptUri, acmeContext.AccountKey, new AcmeHttpClient(letsEncryptUri, new HttpClient()));
            }
            else
            {
                var pemKey = File.ReadAllText(pemKeyFilename);
                var accountKey = KeyFactory.FromPem(pemKey);

                acmeContext = new AcmeContext(letsEncryptUri, accountKey, new AcmeHttpClient(letsEncryptUri, new HttpClient()));
            }

            return acmeContext;
        }
    }
}
