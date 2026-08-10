using Custom.Razor.Sys;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Data.Sys.Wrappers;
using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Dnn.Razor;

[PrivateApi]
internal class DnnRazorHelper() : RazorHelperBase("Sxc.RzrHlp")
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
            .New(new(){ Page = Page, Helper = this, IsSystemAdmin = ExCtx.GetContextOfBlock()?.User.IsSystemAdmin ?? false });
            //.Init(Page, this, ExCtx.GetContextOfBlock()?.User.IsSystemAdmin ?? false);

    #endregion

    #region DynamicModel and Factory

    private ICodeDataPoCoWrapperService CodeDataWrapper => _dynJacketFactory.Get(() => ExCtx.GetService<ICodeDataPoCoWrapperService>());
    private readonly GetOnce<ICodeDataPoCoWrapperService> _dynJacketFactory = new();

    /// <inheritdoc cref="IRazor14{TModel,TServiceKit}.DynamicModel"/>
    public dynamic DynamicModel => _dynamicModel ??= CodeDataWrapper.FromDictionary(Page.PageData);
    private dynamic _dynamicModel;

    internal void SetDynamicModel(RenderSpecs viewData)
    {
        var l = Log.Fn();
        _dynamicModel = CodeDataWrapper.DynamicFromObject(viewData.Data, WrapperSettings.Dyn(children: false, realObjectsToo: false));
        l.Done();
    }

    #endregion
}