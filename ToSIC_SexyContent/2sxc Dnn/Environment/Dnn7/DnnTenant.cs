using System.IO;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Environment;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnTenant: Tenant<PortalSettings>

    {
        public override string DefaultLanguage => Original.DefaultLanguage;

        public override int Id => Original.PortalId;

        public override string Name => Original.PortalName;

        public override string SxcPath => Path.Combine(Original.HomeDirectory, SexyContent.Settings.AppsRootFolder);

        public override bool RefactorUserIsAdmin
            => Original.UserInfo.IsInRole(Original.AdministratorRoleName);

        public override string ContentPath => Original.HomeDirectory;

        public DnnTenant(PortalSettings settings) : base(settings ?? PortalSettings.Current) {}
    }
}