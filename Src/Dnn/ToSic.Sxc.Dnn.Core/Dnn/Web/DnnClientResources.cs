using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;
using System.Web.UI;
using ToSic.Eav;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Dnn.Features;
using ToSic.Sxc.Web.Internal.PageFeatures;
using ToSic.Sxc.Web.Internal.Url;

namespace ToSic.Sxc.Dnn.Web;

internal class DnnClientResources(DnnJsApiHeader dnnJsApiHeader, DnnRequirements dnnRequirements)
    : ServiceBase($"{DnnConstants.LogName}.JsCss", connect: [dnnJsApiHeader, dnnRequirements])
{
    public DnnClientResources Init(Page page, bool? forcePre1025Behavior, IBlockBuilder blockBuilder)
    {
        _forcePre1025Behavior = forcePre1025Behavior;
        _page = page;
        _blockBuilder = blockBuilder;
        return this;
    }
    private IBlockBuilder _blockBuilder;
    private Page _page;
    private bool? _forcePre1025Behavior;

    internal IList<IPageFeature> Features => _features ??= _blockBuilder?.Run(true, specs: new())?.Features ?? new List<IPageFeature>();
    private IList<IPageFeature> _features;

    public IList<IPageFeature> AddEverything(IList<IPageFeature> features = null)
    {
        var l = Log.Fn<IList<IPageFeature>>();
        // temporary solution, till the features are correctly activated in the block
        // auto-detect BlockBuilder params
        features ??= Features;

        // normal scripts
        var editJs = features.Contains(SxcPageFeatures.JsCmsInternal);
        var readJs = features.Contains(SxcPageFeatures.JsCore);
        var editCss = features.Contains(SxcPageFeatures.ToolbarsInternal);

        if (!readJs && !editJs && !editCss && !features.Any())
            return l.Return(features, "nothing to add");

        l.A("user is editor, or template requested js/css, will add client material");

        // register scripts and css
        RegisterClientDependencies(_page, readJs, editJs, editCss, features);

        // New in 11.11.02 - DNN has a strange behavior where the current language isn't known till PreRender
        // so we have to move adding the header to here.
        // MustAddHeaders may have been set earlier by the engine, or now by the various js added
        l.A($"{nameof(MustAddHeaders)}={MustAddHeaders}");
        if (MustAddHeaders) dnnJsApiHeader.AddHeaders();

        return l.ReturnAsOk(features);
    }


    /// <summary>
    /// new in 10.25 - by default jQuery isn't loaded any more
    /// but older razor templates might still expect it
    /// and any other old behaviour, incl. no-view defined, etc. should activate compatibility
    /// </summary>
    public void EnforcePre1025Behavior()
    {
        var l = Log.Fn("Activate Anti-Forgery for compatibility with old behavior");
        ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
        MustAddHeaders = true;
        l.Done();
    }

    /// <summary>
    /// new in 10.25 - by default now jQuery isn't loaded!
    /// but any old behaviour, incl. no-view defined, etc. should activate compatibility
    /// </summary>
    /// <returns></returns>
    public bool NeedsPre1025Behavior() => _forcePre1025Behavior
                                          ?? (dnnRequirements.RequirementsMet() ? (_blockBuilder?.GetEngine() as IEngineDnnOldCompatibility)?.OldAutoLoadJQueryAndRvt : null)
                                          ?? true;


    public void RegisterClientDependencies(Page page, bool readJs, bool editJs, bool editCss, IList<IPageFeature> overrideFeatures = null)
    {
        var l = Log.Fn($"-, {nameof(readJs)}:{readJs}, {nameof(editJs)}:{editJs}, {nameof(editCss)}:{editCss}");
        
        var features = overrideFeatures ?? Features;

        var root = DnnConstants.SysFolderRootVirtual;
        root = page.ResolveUrl(root);
        var ver = EavSystemInfo.VersionWithStartUpBuild;
        const int priority = (int)FileOrder.Js.DefaultPriority - 2;

        // add edit-mode CSS
        if (editCss) RegisterCss(page, $"{root}{SxcPageFeatures.ToolbarsInternal.UrlInDist}");

        // add read-js
        if (readJs || editJs)
        {
            l.A("add $2sxc api and headers");
            RegisterJs(page, ver, $"{root}{SxcPageFeatures.JsCore.UrlInDist}", true, priority);
            MustAddHeaders = true;
        }

        // add edit-js (commands, manage, etc.)
        if (editJs)
        {
            l.A("add 2sxc edit api; also needs anti-forgery");
            // note: the inpage only works if it's not in the head, so we're adding it below
            RegisterJs(page, ver, $"{root}{SxcPageFeatures.JsCmsInternal.UrlInDist}", false, priority + 1);
        }

        if (features.Contains(SxcPageFeatures.JQuery))
            JavaScript.RequestRegistration(CommonJs.jQuery);

        if (features.Contains(SxcPageFeatures.TurnOn))
            RegisterJs(page, ver, $"{root}{SxcPageFeatures.TurnOn.UrlInDist}", true, priority + 10);

        if (features.Contains(SxcPageFeatures.CmsWysiwyg))
            RegisterCss(page, $"{root}{SxcPageFeatures.CmsWysiwyg.UrlInDist}");

        l.Done();
    }


    #region DNN Bug with Current Culture

    // We must add the _js header but we must wait beyond the initial page-load until Pre-Render
    // Because for reasons unknown DNN (at least in V7.4+ but I think also in 9) doesn't have 
    // the right PortalAlias and language set until then. 
    // before that it assumes the PortalAlias is a the default alias, even if the url clearly shows another language

    private bool MustAddHeaders { get; set; }

    #endregion


    #region add scripts / css with bypassing the official ClientResourceManager

    private static void RegisterJs(Page page, string version, string path, bool toHead, int priority)
    {
        if (string.IsNullOrWhiteSpace(path)) return;

        var url = UrlHelpers.QuickAddUrlParameter(path, "v", version);
        if (toHead)
            ClientResourceManager.RegisterScript(page, url, priority, DnnPageHeaderProvider.DefaultName);
        else
            page.ClientScript.RegisterClientScriptInclude(typeof(Page), path, url);
    }

    private static void RegisterCss(Page page, string path)
        => ClientResourceManager.RegisterStyleSheet(page, path);

    #endregion



}