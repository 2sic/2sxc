using System;
using System.Web;
using System.Web.UI;
using ToSic.Eav.Context;
using ToSic.Eav.DI;
using ToSic.Eav.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Services;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Dnn.Code
{
    /// <summary>
    /// Dnn implementation
    /// goal is that we can hook into certain page lifecycle events to ensure changes
    /// happen to the page when necessary
    /// </summary>
    public class DnnDynamicCodeService: DynamicCodeService
    {
        public DnnDynamicCodeService(Dependencies dependencies) : base(dependencies)
        {
            Log.Rename(DnnConstants.LogName + ".DynCdS");
            _pageServiceShared = ScopedServiceProvider.Build<Lazy<PageServiceShared>>();
            _pageChangeSummary = ScopedServiceProvider.Build<Lazy<PageChangeSummary>>();
            _dnnPageChanges = ScopedServiceProvider.Build<Lazy<DnnPageChanges>>();
            _dnnClientResources = ScopedServiceProvider.Build<Lazy<DnnClientResources>>();
            _user = dependencies.User;
            Page = HttpContext.Current?.Handler as Page;

            if (Page != null)
                Page.PreRender += Page_PreRender;
        }
        private readonly Lazy<PageServiceShared> _pageServiceShared;
        private readonly Lazy<PageChangeSummary> _pageChangeSummary;
        private readonly Lazy<DnnPageChanges> _dnnPageChanges;
        private readonly Lazy<DnnClientResources> _dnnClientResources;
        private readonly Lazy<IUser> _user;


        private void Page_PreRender(object sender, EventArgs e)
        {
            var wrapLog = Log.Fn();
            var user = _user.Value;
            var changes = _pageChangeSummary.Value.FinalizeAndGetAllChanges(_pageServiceShared.Value, user.IsSuperUser || user.IsSiteAdmin);
            _dnnPageChanges.Value.Apply(Page, changes);
            var dnnClientResources = _dnnClientResources.Value.Init(Page, false, null, Log);
            dnnClientResources.AddEverything(changes?.Features);
            wrapLog.Done();
        }

        public Page Page;
    }
}
