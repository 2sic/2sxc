using System;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Context;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Context
{
    /// <summary>
    /// A Dnn Page which will be auto-initialized on creation
    /// Important for scenarios where we don't have a module to fill in the details
    /// </summary>
    public class DnnPage: Page
    {
        public DnnPage(Lazy<IHttp> httpLazy) : base(httpLazy)
        {
            InitPageIdAndUrl( PortalSettings.Current?.ActiveTab);
        }

        internal string InitPageIdAndUrl(TabInfo activeTab)
        {
            Init(activeTab?.TabID ?? Eav.Constants.NullId);

            // the FullUrl will throw an error in DNN search scenarios
            try
            {
                // skip during search (usual HttpContext is missing for search)
                if (System.Web.HttpContext.Current != null)
                    Url = activeTab?.FullUrl.TrimLastSlash();
                else
                    return "no http-context, can't add page";
            }
            catch
            {
                /* ignore */
            }

            return Url;
        }

    }
}
