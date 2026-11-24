using Custom.Razor.Sys;
using System.Web.Razor.Parser.SyntaxTree;
using ToSic.Sxc.Code.Context.Sys;
using ToSic.Sxc.Data.Sys.Wrappers;
using ToSic.Sxc.Dnn.Razor.Sys;
using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Dnn.Razor;

[PrivateApi]
internal class DnnRazorHelper() : ServiceBase("Sxc.RzrHlp")
{
    #region Constructor / Init

    public DnnRazorHelper Init(RazorComponentBase page)
    {
        Page = page;
        return this;
    }

    public RazorComponentBase Page { get; private set; }

    #endregion

    #region Error Forwarding

    internal void ConfigurePage(WebPageBase parentPage, string virtualPath)
    {
        // Child pages need to get their context from the Parent
        // ...but we're not quite sure why :) - maybe this isn't actually needed
        Page.Context = parentPage.Context;

        // Return if parent page is not a SexyContentWebPage
        if (parentPage is not RazorComponentBase typedParent)
            return;

        ParentPage = typedParent;

        // Only call the Page.ConnectToRoot, as it will call-back this objects ConnectToRoot
        // So don't call: ConnectToRoot(typedParent.ExCtx);
        Page.ConnectToRoot(typedParent.ExCtx);

        Log.A($"{nameof(virtualPath)} for Render etc.:{virtualPath}");
    }

    internal RazorComponentBase ParentPage { get; set; }

    #endregion

    public ICodeLog CodeLog => field ??= new CodeLog(Log);

    #region Html Helper

    internal IHtmlHelper GetHtml(ICodeContext exCtx) =>
        exCtx.GetService<Generator<HtmlHelper, HtmlHelperContext>>()
            .New(new()
            {
                Page = Page, RazorHelper = this, IsSystemAdmin = exCtx.Block?.Context?.User.IsSystemAdmin ?? false
            });

    #endregion

    #region DynamicModel and Factory

    /// <inheritdoc cref="IRazor14{TModel,TServiceKit}.DynamicModel"/>
    public object GetDynamicModel(ICanGetService exCtx) => _dynamicModel ??= exCtx.GetService<ICodeDataPoCoWrapperService>().FromDictionary(Page.PageData);
    private object _dynamicModel;

    internal void SetDynamicModel(ICanGetService exCtx, RenderSpecs viewData)
    {
        var l = Log.Fn();
        _dynamicModel = exCtx.GetService<ICodeDataPoCoWrapperService>().DynamicFromObject(viewData.Data, WrapperSettings.Dyn(children: false, realObjectsToo: false));
        l.Done();
    }

    #endregion

    #region Exception Forwarding (moved here from RazorHelperBase 2026-08-10 2dm

    public List<Exception>? ExceptionsOrNull { get; private set; }

    public Exception Add(Exception ex)
    {
        (ExceptionsOrNull ??= []).Add(ex);
        return ex;
    }

    #endregion
}