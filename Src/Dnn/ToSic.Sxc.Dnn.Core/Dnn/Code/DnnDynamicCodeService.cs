using System;
using System.Web;
using System.Web.UI;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
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
        public new class ScopedDependencies: ServiceDependencies
        {
            public LazySvc<PageServiceShared> PageServiceShared { get; }
            public LazySvc<PageChangeSummary> PageChangeSummary { get; }
            public LazySvc<DnnPageChanges> DnnPageChanges { get; }
            public LazySvc<DnnClientResources> DnnClientResources { get; }

            public ScopedDependencies(
                LazySvc<PageServiceShared> pageServiceShared,
                LazySvc<PageChangeSummary> pageChangeSummary,
                LazySvc<DnnPageChanges> dnnPageChanges,
                LazySvc<DnnClientResources> dnnClientResources
                )
            {
                AddToLogQueue(
                    PageServiceShared = pageServiceShared,
                    PageChangeSummary = pageChangeSummary,
                    DnnPageChanges = dnnPageChanges,
                    DnnClientResources = dnnClientResources
                );
            }
        }

        public DnnDynamicCodeService(Dependencies dependencies) : base(dependencies)
        {
            Log.Rename(DnnConstants.LogName + ".DynCdS");
            _scopedDeps = ScopedServiceProvider.Build<ScopedDependencies>().SetLog(Log);
            _user = dependencies.User;
            Page = HttpContext.Current?.Handler as Page;

            if (Page != null)
                Page.PreRender += Page_PreRender;
        }

        private readonly ScopedDependencies _scopedDeps;
        private readonly LazySvc<IUser> _user;


        private void Page_PreRender(object sender, EventArgs e)
        {
            var wrapLog = Log.Fn();
            var user = _user.Value;
            var changes = _scopedDeps.PageChangeSummary.Value.FinalizeAndGetAllChanges(_scopedDeps.PageServiceShared.Value, user.IsContentAdmin);
            _scopedDeps.DnnPageChanges.Value.Apply(Page, changes);
            var dnnClientResources = _scopedDeps.DnnClientResources.Value.Init(Page, false, null);
            dnnClientResources.AddEverything(changes?.Features);
            wrapLog.Done();
        }

        public Page Page;
    }
}
