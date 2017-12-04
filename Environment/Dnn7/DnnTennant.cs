using System.IO;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Environment;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnTennant: Tennant<PortalSettings>

    {
        public override string DefaultLanguage => Settings.DefaultLanguage;

        public override int Id => Settings.PortalId;

        public override string RootPath => Path.Combine(Settings.HomeDirectory, SexyContent.Settings.TemplateFolder);

        public DnnTennant(PortalSettings settings) : base(settings) {}
    }
}