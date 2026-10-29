using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using ToSic.Eav.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys.Page;
using ToSic.Sxc.Web.Sys.Http;

namespace ToSic.Sxc.Dnn.Context;

internal class DnnPageFactory(LazySvc<IHttp> httpLazy) : ServiceBase("Dnn.PageFac")
{
    public IPage Create()
        => Create(PortalSettings.Current?.ActiveTab, null);

    public IPage Create(TabInfo? activeTab, int? pageId)
    {
        // Priority
        // 1. PageId if ever provided (only in certain cases in the 
        var id = pageId ?? activeTab?.TabID ?? EavConstants.NullId;
        return new Page(
            Id: id,
            Url: Url(activeTab),
            Parameters: httpLazy.Value.ExtractParameters()
        );
    }

    private static string Url(TabInfo activeTab)
    {
        // the FullUrl will throw an error in DNN search scenarios
        try
        {
            // skip during search (usual HttpContext is missing for search)
            return System.Web.HttpContext.Current != null
                ? activeTab?.FullUrl.TrimLastSlash()
                : "no http-context, can't add page";
        }
        catch
        {
            return "error:retrieving-url";
        }
    }
}