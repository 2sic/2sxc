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
        public DnnTenant(PortalSettings settings): base(GetBestPortalSettings(settings)) {}

        /// <summary>
        /// Very special helper to work around a DNN issue
        /// Reason is that PortalSettings.Current is always "perfect" and also contains root URLs and current Page
        /// Other Portalsettings may not contain this (partially populated objects)
        /// In case we're requesting a DnnTenant with incomplete Portalsettings
        /// we want to correct this here
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private static PortalSettings GetBestPortalSettings(PortalSettings settings)
        {
            // in case we don't have an HTTP Context with current portal settings, don't try anything
            if (PortalSettings.Current == null) return settings;

            // If we don't have settings, or they point to the same portal, then use that
            if (settings == null || settings == PortalSettings.Current || settings.PortalId == PortalSettings.Current.PortalId)
                return PortalSettings.Current;

            // fallback: use supplied settings
            return settings;
        }

        public override string DefaultLanguage => _defaultLanguage ?? (_defaultLanguage = UnwrappedContents.DefaultLanguage.ToLowerInvariant());
        private string _defaultLanguage;

        /// <inheritdoc />
        public override int Id => UnwrappedContents.PortalId;

        /// <inheritdoc />
        public override string Name => UnwrappedContents.PortalName;

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Important special case: if the PortalSettings are not from the PortalSettings.Current, then the
        /// PortalAlias are null!!!
        /// I believe this should only matter in very special cases
        /// Like when showing a module from another portal - in which case we don't need that alias
        /// but the current one. Just keep this in mind in case anything ever breaks.
        /// </remarks>
        public override string Url => UnwrappedContents?.PortalAlias?.HTTPAlias
                                          ?? PortalSettings.Current?.PortalAlias?.HTTPAlias
                                          ?? "err-portal-alias-not-loaded";

        [PrivateApi]
        public override string AppsRoot => Path.Combine(UnwrappedContents.HomeDirectory, Settings.AppsRootFolder);

        [PrivateApi]
        public override bool RefactorUserIsAdmin
            => UnwrappedContents.UserInfo.IsInRole(UnwrappedContents.AdministratorRoleName);

        /// <inheritdoc />
        public override string ContentPath => UnwrappedContents.HomeDirectory;

    }
}