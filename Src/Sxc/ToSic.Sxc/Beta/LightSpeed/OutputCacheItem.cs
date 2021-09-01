using System.Collections.Generic;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.PageFeatures;

namespace ToSic.Sxc.Beta.LightSpeed
{
    public class OutputCacheItem
    {
        public string Html;
        public bool EnforcePre1025 = true;
        public List<IPageFeature> Features;
        public List<ClientAssetInfo> Assets;

    }
}
