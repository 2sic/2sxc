using System.Web;
using System.Web.UI;
using ToSic.Sxc.Dnn.Services.Sys;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Web.Sys.PageService;

namespace ToSic.Sxc.Dnn.Code.Sys;

/// <summary>
/// Dnn implementation
/// goal is that we can hook into certain page lifecycle events,
/// to ensure changes happen to the page when necessary
/// </summary>
partial class DnnDynamicCodeService
{
    private void RegisterPagePreRender() => (HttpContext.Current?.Handler as Page)?.PreRender += Page_PreRender;

    private void Page_PreRender(object sender, EventArgs e)
    {
        if (HttpContext.Current?.Handler is not Page page)
            return;
        var ssp = ScopedServiceProvider;
        var l = Log.Fn();
        var changes = ssp.Build<PageChangeSummary>().FinalizeAndGetAllChanges(
            moduleId: 0, // ignore module Id, we don't expect any caching info here
            ssp.Build<IPageServiceShared>(),
            new(),
            Services.User.IsContentAdmin
        );
        ssp.Build<DnnPageChanges>().Apply(page, changes);

        var dnnClientResources = ssp.Build<DnnClientResources>().Init(page, null);
        dnnClientResources.AddEverything(changes?.Features);
        l.Done();
    }
}