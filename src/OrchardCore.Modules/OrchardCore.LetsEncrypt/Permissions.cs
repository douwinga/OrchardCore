using System.Collections.Generic;
using OrchardCore.Security.Permissions;

namespace OrchardCore.LetsEncrypt
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageLetsEncryptSettings = new Permission("ManageLetsEncryptSettings", "Manage Let's Encrypt Settings");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageLetsEncryptSettings,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype
                {
                    Name = "Administrator",
                    Permissions = new[] { ManageLetsEncryptSettings }
                },
            };
        }
    }
}