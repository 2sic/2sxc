using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Render.Sys.ModuleHtml;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Services.TurnOn.Sys;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Sys.Render.PageFeatures;
using ToSic.Sxc.Web.Sys.ContentSecurityPolicy;

namespace ToSic.Sxc.Services.Page.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class PageService(
    IPageServiceShared pageServiceShared,
    LazySvc<IContentSecurityPolicyService> cspServiceLazy,
    LazySvc<IHtmlTagsService> htmlTagsLazy,
    LazySvc<ITurnOnService> turnOn,
    LazySvc<IModuleHtmlService> moduleService,
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

    /// <summary>
    /// Re-apply cached changes from a render result.
    /// </summary>
    /// <param name="renderResult"></param>
    /// <remarks>
    /// It's implemented on the PageService because it needs some private properties/methods.
    /// </remarks>
    public void ReplaceCachedChanges(RenderResult renderResult)
    {
        var l = Log.Fn();
        if (renderResult.PartialActivateWip?.Any() == true)
            Activate(renderResult.PartialActivateWip.ToArray());

        if (renderResult.FeaturesFromResources is {} list)
            foreach (var ffs in list)
                if (ffs is PageFeatureFromSettings typed)
                    PageServiceShared.PageFeatures.FeaturesFromSettingsAdd(typed);

        foreach (var tagSet in renderResult.PartialModuleTags ?? [])
            moduleService.Value.AddTag(tagSet.Tag, moduleId: ModuleId, noDuplicates: tagSet.NoDuplicates);
        l.Done();
    }
}