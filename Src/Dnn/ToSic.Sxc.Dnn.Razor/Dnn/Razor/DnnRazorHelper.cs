using System.Web.Hosting;
using ToSic.Eav;
using ToSic.Eav.Code.Help;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Data.Internal.Wrapper;
using ToSic.Sxc.Dnn.Code;

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
        Page.ConnectToRoot(typedParent._CodeApiSvc);

        Log.A($"{nameof(virtualPath)} for Render etc.:{virtualPath}");
    }

    internal RazorComponentBase ParentPage { get; set; }

    #endregion

    #region Html Helper

    internal IHtmlHelper Html => _html ??= _CodeApiSvc.GetService<HtmlHelper>().Init(Page, this,
        ((ICodeApiServiceInternal)_CodeApiSvc)._Block?.Context.User.IsSystemAdmin ?? false);
    private IHtmlHelper _html;

    #endregion

    #region RenderPage

    /// <summary>
    /// RenderPage is disabled in Razor12+ to force designers to use Html.Partial
    /// </summary>
    internal HelperResult RenderPageNotSupported()
        => throw new NotSupportedException("RenderPage(...) is not supported in Hybrid Razor. Use Html.Partial(...) instead.");


    #endregion

    #region Create Instance

    protected override string GetCodeNormalizePath(string virtualPath) 
        => Page.NormalizePath(virtualPath);

    protected override object GetCodeCshtml(string path)
    {
        // ReSharper disable once ConvertTypeCheckToNullCheck
        if (Page is not IHasDnn)
            throw new ExceptionWithHelp(new CodeHelp(name: "create-instance-cshtml-only-in-old-code",
                detect: null,
                uiMessage: "CreateInstance(*.cshtml) is not supported in Hybrid Razor. Use .cs files instead."));
        var pageAsCode = WebPageBase.CreateInstanceFromVirtualPath(path);
        var pageAsRcb = pageAsCode as RazorComponentBase;
        pageAsRcb?.RzrHlp.ConfigurePage(Page, pageAsRcb.VirtualPath);
        return pageAsCode;
    }

    protected override string GetCodeFullPathForExistsCheck(string path)
    {
        var l = Log.Fn<string>(path);
        var fullPath = HostingEnvironment.MapPath(path);
        return l.ReturnAndLog(fullPath);
    }

    #endregion

    #region DynamicModel and Factory

    private CodeDataWrapper CodeDataWrapper => _dynJacketFactory.Get(() => _CodeApiSvc.GetService<CodeDataWrapper>());
    private readonly GetOnce<CodeDataWrapper> _dynJacketFactory = new();

    /// <inheritdoc cref="IRazor14{TModel,TServiceKit}.DynamicModel"/>
    public dynamic DynamicModel => _dynamicModel ??= CodeDataWrapper.FromDictionary(Page.PageData);
    private dynamic _dynamicModel;

    internal void SetDynamicModel(object data) =>
        _dynamicModel = CodeDataWrapper.FromObject(data, WrapperSettings.Dyn(children: false, realObjectsToo: false));

    #endregion
}