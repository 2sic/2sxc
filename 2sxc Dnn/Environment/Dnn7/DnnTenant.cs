using System.IO;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Environment;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnTenant: Tenant<PortalSettings>

    {
        public override string DefaultLanguage => Settings.DefaultLanguage;

        public override int Id => Settings.PortalId;

        public override string Name => Settings.PortalName;

        public override string SxcPath => Path.Combine(Settings.HomeDirectory, ToSic.SexyContent.Settings.AppsRootFolder);

        public override bool RefactorUserIsAdmin
            => Settings.UserInfo.IsInRole(Settings.AdministratorRoleName);

        public override string ContentPath => Settings.HomeDirectory;

        public DnnTenant(PortalSettings settings) : base(settings ?? PortalSettings.Current) {}
    }
}