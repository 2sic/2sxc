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
            // 1. Try to add manual resources from WebResources
            // This must happen in the IPageService which is per-module
            // The PageServiceShared cannot do this, because it doesn't have the WebResources which vary by module
            var webRes = WebResources;
            if (webRes != null)
            {
                var keysToRemove = new List<string>();
                foreach (var key in keys)
                {
                    var resConfig = webRes.Get(key) as DynamicEntity;
                    if(resConfig == null) continue;

                    var enabled = resConfig.Get(WebResourceEnabledField) as bool?;
                    if (enabled == false) continue;

                    var html = resConfig.Get(WebResourceHtmlField) as string;
                    if (html == null) continue;

                    // all ok so far
                    keysToRemove.Add(key);
                    PageServiceShared.Features.ManualFeatureAdd(new PageFeature(key, "", "", html: html));
                }

                // drop keys which were already taken care of
                keys = keys.Where(k => !keysToRemove.Contains(k)).ToArray();
            }

            // 2. If any keys are left, they are probably preconfigured keys, so add them now
            if (keys.Any())
                PageServiceShared.Activate(keys);
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
