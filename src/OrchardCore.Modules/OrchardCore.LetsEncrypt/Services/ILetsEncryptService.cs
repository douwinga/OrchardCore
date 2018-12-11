using System.Threading.Tasks;

namespace OrchardCore.LetsEncrypt.Services
{
    public interface ILetsEncryptService
    {
        Task RequestDnsChallengeCertificate(string registrationEmail, string hostName, bool useStaging);
    }
}
