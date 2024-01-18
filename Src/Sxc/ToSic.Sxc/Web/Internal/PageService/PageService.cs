using ToSic.Lib.DI;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

namespace ToSic.Sxc.Web.Internal.PageService;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class PageService: ServiceForDynamicCode, ToSic.Sxc.Services.IPageService // Important: Write with namespace, because it's easy to confuse with IPageService it supports
{

    public PageService(
        PageServiceShared pageServiceShared,
        LazySvc<ContentSecurityPolicyService> cspServiceLazy,
        LazySvc<IHtmlTagsService> htmlTagsLazy,
        LazySvc<ITurnOnService> turnOn,
        LazySvc<IModuleService> moduleService,
        LazySvc<IFeaturesService> features) : base("2sxc.PgeSrv")
    {
        ConnectServices(_cspServiceLazy = cspServiceLazy,
            _htmlTagsLazy = htmlTagsLazy,
            _moduleService = moduleService,
            _turnOn = turnOn,
            PageServiceShared = pageServiceShared,
            _features = features
        );
    }

    private readonly LazySvc<ContentSecurityPolicyService> _cspServiceLazy;
    private readonly LazySvc<IHtmlTagsService> _htmlTagsLazy;
    private readonly LazySvc<IModuleService> _moduleService;
    private readonly LazySvc<ITurnOnService> _turnOn;
    private readonly LazySvc<IFeaturesService> _features;
    public PageServiceShared PageServiceShared { get; }

    ///// <summary>
    ///// How the changes given to this object should be processed.
    ///// </summary>
    //[PrivateApi("not final yet, will probably change")]
    //public PageChangeModes ChangeMode { get; set; } = PageChangeModes.Auto;


    public bool CspIsEnabled => _cspServiceLazy.Value.IsEnabled;

    public bool CspIsEnforced => _cspServiceLazy.Value.IsEnforced;

    public string AddCsp(string name, params string[] values)
    {
        _cspServiceLazy.Value.Add(name, values);
        return "";
    }
}