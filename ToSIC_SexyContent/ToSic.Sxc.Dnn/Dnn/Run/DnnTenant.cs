using System.IO;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    /// <summary>
    /// This is a DNN implementation of a Tenant-object. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class DnnTenant: Tenant<PortalSettings>
    {
        /// <summary>
        /// DI Constructor, will get the current portal settings
        /// #TodoDI not ideal yet, as PortalSettings.current is still retrieved from global
        /// </summary>
        public DnnTenant() : this(PortalSettings.Current) { }

        /// <inheritdoc />
        public DnnTenant(PortalSettings settings) : base(settings ?? PortalSettings.Current) {}

        public override string DefaultLanguage => _defaultLanguage ?? (_defaultLanguage = UnwrappedContents.DefaultLanguage.ToLowerInvariant());
        private string _defaultLanguage;

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

    }
}