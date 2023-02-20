using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web.PageFeatures;
using static ToSic.Eav.Configuration.ConfigurationConstants;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService
    {
        /// <inheritdoc />
        public string Activate(params string[] keys) => Log.Func(() =>
        {
            // 1. Try to add manual resources from WebResources
            // This must happen in the IPageService which is per-module
            // The PageServiceShared cannot do this, because it doesn't have the WebResources which vary by module
            if (!(WebResources is null)) // special problem: DynamicEntity null-compare isn't quite right, **do not** use `!=`
                keys = AddManualResources(keys);

            // 2. If any keys are left, they are probably preconfigured keys, so add them now
            if (keys.Any())
            {
                var added = PageServiceShared.Activate(keys);
                // also add to this specific module, as we need a few module-level features to activate in case...
                _DynCodeRoot?.Block?.BlockFeatureKeys.AddRange(added);
            }

            return ("", "ok");
        });

        /// <inheritdoc />
        public string Activate(
            string noParamOrder = Eav.Parameters.Protector,
            bool condition = true,
            params string[] features) => Log.Func(() =>
        {
            // Check condition - default is true - so if it's false, this overload was called
            if (!condition)
                return ("", "condition false");
            
            // Todo: unclear what to do with the parameter protector
            // Maybe must check the parameter protector, because we're not sure if the user may be calling this overload with a feature name
            // Reason is we're not 100% sure it takes the simple overload vs. this one if 
            // only one string is given, but ATM that's the case.

            return (Activate(features), "condition true, added");
        });

        private string[] AddManualResources(string[] keys)
        {
            var wrapLog = Log.Fn<string[]>();
            var keysToRemove = new List<string>();
            foreach (var key in keys)
            {
                Log.A($"Key: {key}");
                if (!(WebResources.Get(key) is DynamicEntity resConfig)) continue; // special problem: DynamicEntity null-compare isn't quite right, don't! use ==

                var enabled = resConfig.Get(WebResourceEnabledField) as bool?;
                if (enabled == false) continue;

                if (!(resConfig.Get(WebResourceHtmlField) is string html)) continue;

                Log.A("Found html and everything, will register");
                // all ok so far
                keysToRemove.Add(key);
                PageServiceShared.PageFeatures.FeaturesFromSettingsAdd(new PageFeatureFromSettings(key, "", "", html: html));
            }

            // drop keys which were already taken care of
            keys = keys.Where(k => !keysToRemove.Contains(k)).ToArray();
            return wrapLog.Return(keys);
        }

        private DynamicEntity WebResources => _webResources.Get(() => (_DynCodeRoot?.Settings as DynamicStack)?.Get(WebResourcesNode) as DynamicEntity);
        private readonly GetOnce<DynamicEntity> _webResources = new GetOnce<DynamicEntity>();
        
    }
}
