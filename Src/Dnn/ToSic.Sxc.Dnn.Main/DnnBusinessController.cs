using DotNetNuke.Entities.Modules;
using ToSic.Sxc.Dnn.StartUp;

// Note about the name
// Some day we should change this namespace to ToSic.Sxc.Dnn.something
// But we can't just do it, because the name is registered in Dnn DBs, so update-scripts would be needed
// TODO: STV - WHY IS THIS NOT PART OF THE DnnBusinessController? it seems that that is already the term in the DB?

// ReSharper disable once CheckNamespace
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