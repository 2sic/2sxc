using DotNetNuke.Entities.Modules;
using ToSic.Sxc.Dnn.StartUp;

// Note about the name
// Some day we should change this namespace to ToSic.Sxc.Dnn.something
// But we can't just do it, because the name is registered in Dnn DBs, so update-scripts would be needed
// WHY IS THIS NOT PART OF THE DnnBusinessController? it seems that that is already the term in the DB?
// Reason if that can't call StartupDnn().Configure() from ToSic.Sxc.Dnn.DnnBusinessController because of circular dependency
// and need to configure DI before UpgradeModule to fix issue: "Module upgrade did not complete."
// "System.ArgumentNullException: Value cannot be null. Parameter name: provider
// at Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService[T](IServiceProvider provider)
// at ToSic.Eav.Factory.GetServiceProvider()"
// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DnnBusinessController: ToSic.Sxc.Dnn.DnnBusinessController, IUpgradeable, IVersionable
{
    public new string UpgradeModule(string version)
    {
        new StartupDnn().Configure(); // can't call it from ToSic.Sxc.Dnn.DnnBusinessController because of circular dependency
        return base.UpgradeModule(version);
    }
}