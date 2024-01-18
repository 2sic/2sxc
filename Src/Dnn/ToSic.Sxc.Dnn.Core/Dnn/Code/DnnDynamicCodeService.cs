using System.Web;
using System.Web.UI;
using ToSic.Eav.Context;
using ToSic.Lib.Services;
using ToSic.Sxc.Dnn.Services;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Dnn.Code;

/// <summary>
/// Dnn implementation
/// goal is that we can hook into certain page lifecycle events to ensure changes
/// happen to the page when necessary
/// </summary>
internal class DnnDynamicCodeService: DynamicCodeService
{
    public new class MyScopedServices: MyServicesBase
    {
        public LazySvc<PageServiceShared> PageServiceShared { get; }
        public LazySvc<PageChangeSummary> PageChangeSummary { get; }
        public LazySvc<DnnPageChanges> DnnPageChanges { get; }
        public LazySvc<DnnClientResources> DnnClientResources { get; }

        public MyScopedServices(
            LazySvc<PageServiceShared> pageServiceShared,
            LazySvc<PageChangeSummary> pageChangeSummary,
            LazySvc<DnnPageChanges> dnnPageChanges,
            LazySvc<DnnClientResources> dnnClientResources
        )
        {
            ConnectServices(
                PageServiceShared = pageServiceShared,
                PageChangeSummary = pageChangeSummary,
                DnnPageChanges = dnnPageChanges,
                DnnClientResources = dnnClientResources
            );
        }
    }

    public DnnDynamicCodeService(MyServices services) : base(services, $"{DnnConstants.LogName}.DynCdS")
    {
        _scopedServices = ScopedServiceProvider.Build<MyScopedServices>().ConnectServices(Log);
        _user = services.User;
        Page = HttpContext.Current?.Handler as Page;

        if (Page != null)
            Page.PreRender += Page_PreRender;
    }

    private readonly MyScopedServices _scopedServices;
    private readonly LazySvc<IUser> _user;


    private void Page_PreRender(object sender, EventArgs e) => Log.Do(() =>
    {
        var user = _user.Value;
        var changes = _scopedServices.PageChangeSummary.Value.FinalizeAndGetAllChanges(
            _scopedServices.PageServiceShared.Value, user.IsContentAdmin);
        _scopedServices.DnnPageChanges.Value.Apply(Page, changes);
        var dnnClientResources = _scopedServices.DnnClientResources.Value.Init(Page, false, null);
        dnnClientResources.AddEverything(changes?.Features);
    });

    public Page Page;
}