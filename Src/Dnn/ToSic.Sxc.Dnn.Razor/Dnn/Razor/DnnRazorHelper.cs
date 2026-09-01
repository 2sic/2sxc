using Custom.Razor.Sys;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Data.Sys.Wrappers;
using ToSic.Sxc.Dnn.Razor.Sys;
using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Dnn.Razor;

[PrivateApi]
internal class DnnRazorHelper() : CodeHelperBase("Sxc.RzrHlp")
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
        if (parentPage is not RazorComponentBase typedParent) return;

        ParentPage = typedParent;

        // Only call the Page.ConnectToRoot, as it will call-back this objects ConnectToRoot
        // So don't call: ConnectToRoot(typedParent._DynCodeRoot);
        Page.ConnectToRoot(typedParent.ExCtx);

        Log.A($"{nameof(virtualPath)} for Render etc.:{virtualPath}");
    }

    internal RazorComponentBase ParentPage { get; set; }

    #endregion

    #region Html Helper

    internal IHtmlHelper Html => field
        ??= ExCtx.GetService<Generator<HtmlHelper, HtmlHelperContext>>()
            .New(new(){ Page = Page, RazorHelper = this, IsSystemAdmin = ExCtx.GetContextOfBlock()?.User.IsSystemAdmin ?? false });
            //.Init(Page, this, ExCtx.GetContextOfBlock()?.User.IsSystemAdmin ?? false);

    #endregion

    #region DynamicModel and Factory

    private ICodeDataPoCoWrapperService CodeDataWrapper => field ??= ExCtx.GetService<ICodeDataPoCoWrapperService>();

    /// <inheritdoc cref="IRazor14{TModel,TServiceKit}.DynamicModel"/>
    public object DynamicModel => _dynamicModel ??= CodeDataWrapper.FromDictionary(Page.PageData);
    private object _dynamicModel;

    internal void SetDynamicModel(RenderSpecs viewData)
    {
        var l = Log.Fn();
        _dynamicModel = CodeDataWrapper.DynamicFromObject(viewData.Data, WrapperSettings.Dyn(children: false, realObjectsToo: false));
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