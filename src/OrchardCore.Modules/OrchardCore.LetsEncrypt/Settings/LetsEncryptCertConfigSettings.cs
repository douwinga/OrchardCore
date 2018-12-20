namespace OrchardCore.LetsEncrypt.Settings
{
    public class LetsEncryptCertConfigSettings
    {
        public string PfxPassword { get; set; }
        public string Country { get; set; }
        public string StateOrProvince { get; set; }
        public string Locality { get; set; }
        public string Organization { get; set; }
        public string OrganizationUnit { get; set; }
    }
}
