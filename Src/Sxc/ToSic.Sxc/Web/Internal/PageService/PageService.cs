using ToSic.Lib.DI;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

namespace ToSic.Sxc.Web.Internal.PageService;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class PageService(
    PageServiceShared pageServiceShared,
    LazySvc<ContentSecurityPolicyService> cspServiceLazy,
    LazySvc<IHtmlTagsService> htmlTagsLazy,
    LazySvc<ITurnOnService> turnOn,
    LazySvc<IModuleService> moduleService,
    LazySvc<IFeaturesService> features)
    : ServiceForDynamicCode("2sxc.PgeSrv",
            connect: [cspServiceLazy, htmlTagsLazy, moduleService, turnOn, pageServiceShared, features]),
        ToSic.Sxc.Services.IPageService // Important: Write with namespace, because it's easy to confuse with IPageService it supports
{
    public PageServiceShared PageServiceShared { get; } = pageServiceShared;

    ///// <summary>
    ///// How the changes given to this object should be processed.
    ///// </summary>
    //[PrivateApi("not final yet, will probably change")]
    //public PageChangeModes ChangeMode { get; set; } = PageChangeModes.Auto;


    public bool CspIsEnabled => cspServiceLazy.Value.IsEnabled;

    public bool CspIsEnforced => cspServiceLazy.Value.IsEnforced;

    public string AddCsp(string name, params string[] values)
    {
        cspServiceLazy.Value.Add(name, values);
        return "";
    }
}