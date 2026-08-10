using System.Web.Hosting;
using Custom.Razor.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Data.Sys.Wrappers;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Code.Help;
using ToSic.Sys.Exceptions;

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

    #region Create Instance

    private object GetCodeCshtml(string path)
    {
        // ReSharper disable once ConvertTypeCheckToNullCheck
        if (Page is not IHasDnn)
            throw new ExceptionWithHelp(new CodeHelp
            {
                Name = "create-instance-cshtml-only-in-old-code",
                Detect = null,
                UiMessage = "CreateInstance(*.cshtml) is not supported in Hybrid Razor. Use .cs files instead."
            });
        var pageAsCode = WebPageBase.CreateInstanceFromVirtualPath(path);
        var pageAsRcb = pageAsCode as RazorComponentBase;
        pageAsRcb?.RzrHlp.ConfigurePage(Page, pageAsRcb.VirtualPath);
        return pageAsCode;
    }

    private string GetCodeFullPathForExistsCheck(string path)
    {
        var l = Log.Fn<string>(path);
        var fullPath = HostingEnvironment.MapPath(path);
        return l.ReturnAndLog(fullPath);
    }

    #endregion

    #region GetCode

    public object? GetCode(string path, NoParamOrder npo = default, string? className = default)
        => GetCode(path, npo: npo, name: className, throwOnError: true);


    public object? CreateInstance(string virtualPath,
        NoParamOrder npo = default,
        string? name = null,
        string? relativePath = null,
        bool throwOnError = true
    ) => GetCode(virtualPath: virtualPath, npo: npo, name: name, throwOnError: throwOnError);


    /// <summary>
    /// Creates instances of the shared pages with the given relative path
    /// </summary>
    /// <returns></returns>
    protected object? GetCode(string virtualPath,
        NoParamOrder npo,
        string? name,
        bool throwOnError)
    {
        // Note: Don't do parameter checks, as they have already been done
        // and the warnings are a bit different depending on the public signature

        var l = Log.Fn<object?>($"'{virtualPath}', '{name}'");

        if (virtualPath.IsEmptyOrWs())
            return !throwOnError
                ? null
                : throw l.Done(new ArgumentException("path can't be empty"));

        var path = ResolvePathIfAbsoluteToApp(virtualPath)?.ForwardSlash().PrefixSlash()
                   ?? Page.NormalizePath(virtualPath);

        if (!File.Exists(GetCodeFullPathForExistsCheck(path)))
            return !throwOnError
                ? null
                : throw l.Done(new FileNotFoundException("The file does not exist.", path));

        try
        {
            object? result = path.EndsWith(SourceCodeConstants.CsFileExtension)
                ? ExCtx.GetDynamicApi().CreateInstance(path, npo, name: name, relativePath: null, throwOnError: throwOnError)
                : GetCodeCshtml(path);
            return l.Return(result, "ok");
        }
        catch (Exception ex)
        {
            l.Done(ex);
            if (throwOnError)
                throw;
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="overrideRootExCtx">Insert another code Root, ATM a patch for Oqtane Razor</param>
    /// <returns></returns>
    private string? ResolvePathIfAbsoluteToApp(string? path, IExecutionContext? overrideRootExCtx = default)
    {
        var l = Log.Fn<string>(path);
        if (path == null || (!path.StartsWith("/") && !path.StartsWith("\\")))
            return l.ReturnNull("not absolute, return null");

        l.A("Will try to use absolute path relative to the app.");

        if (!path.EndsWith(SourceCodeConstants.CsFileExtension))
            throw l.Done(new ArgumentException("Only '.cs' file paths can start with a slash"));
        var app = (overrideRootExCtx ?? ExCtxOrNull)?.GetTypedApi()?.AppTyped
                  ?? throw l.Done(new Exception("Absolute paths require an App, which was null"));
        var appFolder = app.Folder?.Path
                        ?? throw l.Done(new Exception("Absolute paths require the App folder, which was null"));
        return l.ReturnAndLog(Path.Combine(appFolder, path.TrimPrefixSlash()));
    }


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