﻿using ToSic.Lib.DI;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Web.Sys.ContentSecurityPolicy;

namespace ToSic.Sxc.Services.Page.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class PageService(
    IPageServiceShared pageServiceShared,
    LazySvc<IContentSecurityPolicyService> cspServiceLazy,
    LazySvc<IHtmlTagsService> htmlTagsLazy,
    LazySvc<ITurnOnService> turnOn,
    LazySvc<IModuleService> moduleService,
    LazySvc<IFeaturesService> featuresSvc)
    : ServiceWithContext("2sxc.PgeSrv",
            connect: [cspServiceLazy, htmlTagsLazy, moduleService, turnOn, pageServiceShared, featuresSvc]),
        IPageService // Important: Write with namespace, because it's easy to confuse with IPageService it supports
{
    public IPageServiceShared PageServiceShared { get; } = pageServiceShared;

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