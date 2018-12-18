using System.Threading.Tasks;

namespace OrchardCore.LetsEncrypt.Services
{
    public interface ILetsEncryptService
    {
        Task RequestHttpChallengeCertificate(string registrationEmail, string[] hostnames, bool useStaging);
    }
}
