using System.Threading.Tasks;
using Certes;
using Certes.Acme;

namespace OrchardCore.LetsEncrypt.Services
{
    public class LetsEncryptService : ILetsEncryptService
    {
        public async Task RequestDnsChallengeCertificate(string registrationEmail, string hostName, bool useStaging)
        {
            var acmeContext = await GetOrCreateAcmeContext(registrationEmail, useStaging);
        }

        private async Task<AcmeContext> GetOrCreateAcmeContext(string registrationEmail, bool useStaging)
        {
            // TODO: Use filesystem to store the pem and retrieve if it already exists. Store in db?

            var acme = new AcmeContext(useStaging ? WellKnownServers.LetsEncryptStagingV2 : WellKnownServers.LetsEncryptV2);
            var account = await acme.NewAccount(registrationEmail, true);

            // Save the account key for later use
            var pemKey = acme.AccountKey.ToPem();

            return acme;
        }
    }
}
