using ToSic.Razor.Blade;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Render.Sys.ModuleHtml;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Services.TurnOn.Sys;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Web.Sys.ContentSecurityPolicy;
using ToSic.Sxc.Web.Sys.PageServiceShared;

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

    #region Experimental Listeners

    /// <summary>
    /// Listeners are added / removed through the depth of render-in-render,
    /// so that certain segments can detect what was modified for them and things underneath.
    /// This is to cache changes for replace later on without re-running the razor render.
    /// </summary>
    public PageChangeListenerManagerWip Listeners { get; } = new();

    #endregion

    /// <summary>
    /// Re-apply cached changes from a render result.
    /// </summary>
    /// <param name="renderResult"></param>
    /// <remarks>
    /// It's implemented on the PageService because it needs some private properties/methods.
    /// </remarks>
    public void ReplayCachedChanges(RenderResult renderResult)
    {
        var l = Log.Fn();
        if (renderResult.PartialActivateWip?.Any() == true)
            Activate(renderResult.PartialActivateWip.ToArray());

        foreach (var ffs in renderResult.FeaturesFromResources ?? [])
            PageServiceShared.PageFeatures.FeaturesFromSettingsAdd(ffs);

        foreach (var tagSet in renderResult.PartialModuleTags ?? [])
            moduleService.Value.AddTag(tagSet.Tag, moduleId: ModuleId, noDuplicates: tagSet.NoDuplicates);

        if (renderResult.HeadChanges != null)
            PageServiceShared.Headers.AddRange(renderResult.HeadChanges);

        if (renderResult.PageChanges != null)
            ((PageServiceShared)PageServiceShared).PropertyChanges.AddRange(renderResult.PageChanges);

        l.Done();
    }
}