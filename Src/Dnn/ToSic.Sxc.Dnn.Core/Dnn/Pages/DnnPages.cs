using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using ToSic.Lib.Services;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Dnn.Pages;

/// <summary>
/// Temporary solutions - minor service which ATM is only used in WebAPI.
/// Goal is that this will be more standardized and work across all platforms.
/// So when we do that, this should implement that interface and become internal.
/// </summary>
internal class DnnPages(ILog parentLog) : HelperBase(parentLog, "Dnn.Pages")
{
    internal List<ModuleWithContent> AllModulesWithContent(int portalId)
    {
        var l = Log.Fn<List<ModuleWithContent>>($"{portalId}");
        var mc = ModuleController.Instance;
        var tabC = TabController.Instance;

        // create an array with all modules
        var modules2Sxc = mc.GetModulesByDefinition(portalId, DnnConstants.ModuleNameContent)
            .ToArray()
            .Cast<ModuleInfo>()
            .ToList();
        var dnnMod2SxcApp = mc.GetModulesByDefinition(portalId, DnnConstants.ModuleNameApp)
            .ToArray()
            .Cast<ModuleInfo>()
            .ToList();
        var all = modules2Sxc.Union(dnnMod2SxcApp).ToList();
        Log.A($"Mods for Content: {modules2Sxc.Count}, App: {dnnMod2SxcApp.Count}, Total: {all.Count}");

        // filter the results
        var allMods = all
            .Where(m => m.DefaultLanguageModule == null)
            .Where(m => m.ModuleSettings.ContainsKey(ModuleSettingNames.ContentGroup))
            .ToList();

        var result = allMods.Select(m => new ModuleWithContent
            {
                Module = m,
                ContentGroup = Guid.TryParse(m.ModuleSettings[ModuleSettingNames.ContentGroup].ToString(),
                    out var g)
                    ? g
                    : Guid.Empty,
                Page = tabC.GetTab(m.TabID, portalId)
            })
            .Where(set => set.ContentGroup != Guid.Empty)
            .ToList();

        return l.Return(result, $"{allMods.Count}");
    }
}