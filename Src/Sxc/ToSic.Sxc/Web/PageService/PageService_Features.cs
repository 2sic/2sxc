using System.Collections.Generic;
using System.Linq;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web.PageFeatures;
using static ToSic.Eav.Configuration.ConfigurationConstants;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService
    {
        /// <inheritdoc />
        public void Activate(params string[] keys)
        {
            var wrapLog = Log.Call();

            // 1. Try to add manual resources from WebResources
            // This must happen in the IPageService which is per-module
            // The PageServiceShared cannot do this, because it doesn't have the WebResources which vary by module
            if (!(WebResources is null)) // special problem: DynamicEntity null-compare isn't quite right, don't! use !=
                keys = AddManualResources(keys);

            // 2. If any keys are left, they are probably preconfigured keys, so add them now
            if (keys.Any())
                PageServiceShared.Activate(keys);
            
            wrapLog(null);
        }

        private string[] AddManualResources(string[] keys)
        {
            var wrapLog = Log.Call<string[]>();
            var keysToRemove = new List<string>();
            foreach (var key in keys)
            {
                Log.Add($"Key: {key}");
                var resConfig = WebResources.Get(key) as DynamicEntity;
                if (resConfig is null) continue; // special problem: DynamicEntity null-compare isn't quite right, don't! use ==

                var enabled = resConfig.Get(WebResourceEnabledField) as bool?;
                if (enabled == false) continue;

                var html = resConfig.Get(WebResourceHtmlField) as string;
                if (html == null) continue;

                Log.Add("Found html and everything, will register");
                // all ok so far
                keysToRemove.Add(key);
                PageServiceShared.Features.ManualFeatureAdd(new PageFeature(key, "", "", html: html));
            }

            // drop keys which were already taken care of
            keys = keys.Where(k => !keysToRemove.Contains(k)).ToArray();
            return wrapLog(null, keys);
        }

        private DynamicEntity WebResources
        {
            get
            {
                if (_alreadyTriedToFindWebResources) return _webResources;
                _webResources = (CodeRoot?.Settings as DynamicStack)?.Get(WebResourcesNode) as DynamicEntity;
                _alreadyTriedToFindWebResources = true;
                return _webResources;
            }
        }
        private DynamicEntity _webResources;
        private bool _alreadyTriedToFindWebResources;
    }
}
