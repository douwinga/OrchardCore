using System.Threading.Tasks;
using OrchardCore.LetsEncrypt.Models;

namespace OrchardCore.LetsEncrypt.Services
{
    public interface IAzureCertificateService
    {
        Task InstallAsync(CertificateInstallModel certInstallModel);
    }
}
