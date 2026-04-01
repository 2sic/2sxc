using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;
using DotNetNuke.Web.MvcPipeline.ModuleControl.Page;
using System.Web;
using System.Web.UI;
using ToSic.Eav.Sys;
using ToSic.Sxc.Dnn.Features;
using ToSic.Sxc.Render.Sys.RenderBlock;
using ToSic.Sxc.Sys.Render.PageFeatures;
using ToSic.Sxc.Web.Sys.Url;

namespace ToSic.Sxc.Dnn.Web;

internal class DnnClientResources(DnnJsApiHeader dnnJsApiHeader, DnnRequirements dnnRequirements)
    : ServiceBase($"{DnnConstants.LogName}.JsCss", connect: [dnnJsApiHeader, dnnRequirements])
{
    public DnnClientResources Init(Page page, IBlockBuilder blockBuilder = null)
    {
        _page = page;
        _blockBuilder = blockBuilder;
        return this;
    }

    private IBlockBuilder _blockBuilder;
    private Page _page;

    internal IList<IPageFeature> Features => field ??= _blockBuilder?.Run(true, specs: new())?.Features ?? [];

    public IList<IPageFeature> AddEverything(IList<IPageFeature> features = null)
    {
        var l = Log.Fn<IList<IPageFeature>>();
        features ??= Features;

        var (readJs, editJs, editCss) = FindNeededResources(features);
        if (!readJs && !editJs && !editCss && !features.Any())
            return l.Return(features, "nothing to add");

        l.A("user is editor, or template requested js/css, will add client material");
        RegisterClientDependencies(_page, readJs, editJs, editCss, features);
        AddJsApiHeadersIfNeeded(l);
        return l.ReturnAsOk(features);
    }

    public IList<IPageFeature> AddEverythingMvc(PageConfigurationContext pageContext, IList<IPageFeature> features = null)
    {
        var l = Log.Fn<IList<IPageFeature>>();
        features ??= Features;

        var (readJs, editJs, editCss) = FindNeededResources(features);
        if (!readJs && !editJs && !editCss && !features.Any())
            return l.Return(features, "nothing to add");

        l.A("MVC pipeline page requested js/css, will add client material");
        RegisterClientDependencies(pageContext, readJs, editJs, editCss, features);
        AddJsApiHeadersIfNeeded(l);
        return l.ReturnAsOk(features);
    }

    private void AddJsApiHeadersIfNeeded(ILogCall l)
    {
        l.A($"{nameof(MustAddHeaders)}={MustAddHeaders}");
        if (MustAddHeaders)
            dnnJsApiHeader.AddHeaders();
    }

    private static (bool ReadJs, bool EditJs, bool EditCss) FindNeededResources(IList<IPageFeature> features)
    {
        var editJs = features.Contains(SxcPageFeatures.JsCmsInternal);
        var readJs = features.Contains(SxcPageFeatures.JsCore);
        var editCss = features.Contains(SxcPageFeatures.ToolbarsInternal);
        return (readJs, editJs, editCss);
    }

    public void RegisterClientDependencies(Page page, bool readJs, bool editJs, bool editCss, IList<IPageFeature> overrideFeatures = null)
    {
        var l = Log.Fn($"-, {nameof(readJs)}:{readJs}, {nameof(editJs)}:{editJs}, {nameof(editCss)}:{editCss}");

        var features = overrideFeatures ?? Features;
        var root = page.ResolveUrl(DnnConstants.SysFolderRootVirtual);
        var ver = EavSystemInfo.VersionWithStartUpBuild;
        const int priority = (int)FileOrder.Js.DefaultPriority - 2;

        if (editCss) RegisterCss(page, $"{root}{SxcPageFeatures.ToolbarsInternal.UrlInDist}");

        if (readJs || editJs)
        {
            l.A("add $2sxc api and headers");
            RegisterJs(page, ver, $"{root}{SxcPageFeatures.JsCore.UrlInDist}", true, priority);
            MustAddHeaders = true;
        }

        if (editJs)
        {
            l.A("add 2sxc edit api; also needs anti-forgery");
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

    public void RegisterClientDependencies(PageConfigurationContext pageContext, bool readJs, bool editJs, bool editCss, IList<IPageFeature> overrideFeatures = null)
    {
        var l = Log.Fn($"mvc, {nameof(readJs)}:{readJs}, {nameof(editJs)}:{editJs}, {nameof(editCss)}:{editCss}");

        var features = overrideFeatures ?? Features;
        var root = VirtualPathUtility.ToAbsolute(DnnConstants.SysFolderRootVirtual);
        var ver = EavSystemInfo.VersionWithStartUpBuild;
        const int priority = (int)FileOrder.Js.DefaultPriority - 2;

        if (editCss) RegisterCss(pageContext, $"{root}{SxcPageFeatures.ToolbarsInternal.UrlInDist}");

        if (readJs || editJs)
        {
            l.A("add $2sxc api and MVC services-framework support");
            pageContext.ServicesFramework?.RequestAjaxScriptSupport();
            RegisterJs(pageContext, ver, $"{root}{SxcPageFeatures.JsCore.UrlInDist}", true, priority);
            MustAddHeaders = true;
        }

        if (editJs)
        {
            l.A("add 2sxc edit api and anti-forgery support");
            pageContext.ServicesFramework?.RequestAjaxAntiForgerySupport();
            RegisterJs(pageContext, ver, $"{root}{SxcPageFeatures.JsCmsInternal.UrlInDist}", false, priority + 1);
        }

        if (features.Contains(SxcPageFeatures.JQuery))
            pageContext.JavaScriptLibraryHelper?.RequestRegistration(CommonJs.jQuery);

        if (features.Contains(SxcPageFeatures.TurnOn))
            RegisterJs(pageContext, ver, $"{root}{SxcPageFeatures.TurnOn.UrlInDist}", true, priority + 10);

        if (features.Contains(SxcPageFeatures.CmsWysiwyg))
            RegisterCss(pageContext, $"{root}{SxcPageFeatures.CmsWysiwyg.UrlInDist}");

        l.Done();
    }

    #region DNN Bug with Current Culture

    private bool MustAddHeaders { get; set; }

    #endregion

    #region add scripts / css with bypassing the official ClientResourceManager

    private static void RegisterJs(Page page, string version, string path, bool toHead, int priority)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        var url = UrlHelpers.QuickAddUrlParameter(path, "v", version);
        if (toHead)
            ClientResourceManager.RegisterScript(page, url, priority, DnnPageHeaderProvider.DefaultName);
        else
            page.ClientScript.RegisterClientScriptInclude(typeof(Page), path, url);
    }

    private static void RegisterJs(PageConfigurationContext pageContext, string version, string path, bool _toHead, int _priority)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        var url = UrlHelpers.QuickAddUrlParameter(path, "v", version);
        var script = pageContext.ClientResourceController?.CreateScript(url);
        if (script == null)
            return;

        pageContext.ClientResourceController.AddScript(script);
    }

    private static void RegisterCss(Page page, string path)
        => ClientResourceManager.RegisterStyleSheet(page, path);

    private static void RegisterCss(PageConfigurationContext pageContext, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        var stylesheet = pageContext.ClientResourceController?.CreateStylesheet(path);
        if (stylesheet == null)
            return;

        pageContext.ClientResourceController.AddStylesheet(stylesheet);
    }

    #endregion
}
