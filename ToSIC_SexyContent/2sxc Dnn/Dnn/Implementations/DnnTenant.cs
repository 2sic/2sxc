using System.IO;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Documentation;
using ToSic.Eav.Environment;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// This is a DNN implementation of a Tenant-object. 
    /// </summary>
    [PublicApi]
    public class DnnTenant: Tenant<PortalSettings>
    {
        /// <inheritdoc />
        public override string DefaultLanguage => Original.DefaultLanguage;

        /// <inheritdoc />
        public override int Id => Original.PortalId;

        /// <inheritdoc />
        public override string Name => Original.PortalName;

        [PrivateApi]
        public override string SxcPath => Path.Combine(Original.HomeDirectory, Settings.AppsRootFolder);

        [PrivateApi]
        public override bool RefactorUserIsAdmin
            => Original.UserInfo.IsInRole(Original.AdministratorRoleName);

        /// <inheritdoc />
        public override string ContentPath => Original.HomeDirectory;

        public DnnTenant(PortalSettings settings) : base(settings ?? PortalSettings.Current) {}
    }
}