using System.IO;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    /// <summary>
    /// This is a DNN implementation of a Tenant-object. 
    /// </summary>
    [PublicApi]
    public class DnnTenant: Tenant<PortalSettings>
    {
        /// <inheritdoc />
        public override string DefaultLanguage => UnwrappedContents.DefaultLanguage;

        /// <inheritdoc />
        public override int Id => UnwrappedContents.PortalId;

        /// <inheritdoc />
        public override string Name => UnwrappedContents.PortalName;

        [PrivateApi]
        public override string SxcPath => Path.Combine(UnwrappedContents.HomeDirectory, Settings.AppsRootFolder);

        [PrivateApi]
        public override bool RefactorUserIsAdmin
            => UnwrappedContents.UserInfo.IsInRole(UnwrappedContents.AdministratorRoleName);

        /// <inheritdoc />
        public override string ContentPath => UnwrappedContents.HomeDirectory;

        public DnnTenant(PortalSettings settings) : base(settings ?? PortalSettings.Current) {}
    }
}