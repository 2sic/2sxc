using DotNetNuke.Entities.Modules;

namespace ToSic.SexyContent
{
    public class DnnBusinessController: ToSic.Sxc.Dnn.DnnBusinessController, IUpgradeable, IVersionable
    {
        public new string UpgradeModule(string version)
        {
            new StartupDnn().Configure();
            return base.UpgradeModule(version);
        }
    }
}