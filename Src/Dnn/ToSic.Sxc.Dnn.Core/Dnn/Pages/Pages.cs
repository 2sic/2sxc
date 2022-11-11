using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Context;

namespace ToSic.Sxc.Dnn.Pages
{
    public class Pages: HasLog
    {
        public Pages(ILog parentLog = null) : base("Dnn.Pages", parentLog)
        {
        }

        public List<ModuleWithContent> AllModulesWithContent(int portalId)
        {
            var wrapLog = Log.Fn<List<ModuleWithContent>>($"{portalId}");
            var mc = ModuleController.Instance;
            var tabC = TabController.Instance;

            // create an array with all modules
            var modules2Sxc = mc.GetModulesByDefinition(portalId, DnnConstants.ModuleNameContent).ToArray().Cast<ModuleInfo>().ToList();
            var dnnMod2SxcApp = mc.GetModulesByDefinition(portalId, DnnConstants.ModuleNameApp).ToArray().Cast<ModuleInfo>().ToList();
            var all = modules2Sxc.Union(dnnMod2SxcApp).ToList();
            Log.A($"Mods for Content: {modules2Sxc.Count}, App: {dnnMod2SxcApp.Count}, Total: {all.Count}");

            // filter the results
            var allMods = all
                .Where(m => m.DefaultLanguageModule == null)
                .Where(m => m.ModuleSettings.ContainsKey(Settings.ModuleSettingContentGroup))
                .ToList();

            var result = allMods.Select(m => new ModuleWithContent
            {
                    Module = m,
                    ContentGroup = Guid.TryParse(m.ModuleSettings[Settings.ModuleSettingContentGroup].ToString(),
                        out var g)
                        ? g
                        : Guid.Empty,
                    Page = tabC.GetTab(m.TabID, portalId)
                })
                .Where(set => set.ContentGroup != Guid.Empty)
                .ToList();

            return wrapLog.Return(result, $"{allMods.Count}");
        }
    }
}
