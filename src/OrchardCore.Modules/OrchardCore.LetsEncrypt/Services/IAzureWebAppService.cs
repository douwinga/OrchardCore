using System.Threading.Tasks;

namespace OrchardCore.LetsEncrypt.Services
{
    public interface IAzureWebAppService
    {
        Task InstallCertificate();
    }
}
