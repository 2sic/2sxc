using System.Web.Hosting;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Code.Help;
using ToSic.Sys.Exceptions;

namespace ToSic.Sxc.Dnn.Razor;

/// <summary>
/// Helper to isolate GetCode / CreateInstance for DNN Razor.
/// This is old functionality, so it's separated out.
/// </summary>
[PrivateApi]
internal class DnnRazorGetCodeHelper(RazorComponentBase page, IExecutionContext exCtx) :HelperBase((page as IHasLog)?.Log, "Sxc.RzrHlp")
{
    #region Create Instance

    private object GetCodeCshtml(string path)
    {
        // ReSharper disable once ConvertTypeCheckToNullCheck
        if (page is not IHasDnn)
            throw new ExceptionWithHelp(new CodeHelp
            {
                Name = "create-instance-cshtml-only-in-old-code",
                Detect = null,
                UiMessage = "CreateInstance(*.cshtml) is not supported in Hybrid Razor. Use .cs files instead."
            });
        var pageAsCode = WebPageBase.CreateInstanceFromVirtualPath(path);
        var pageAsRcb = pageAsCode as RazorComponentBase;
        pageAsRcb?.RzrHlp.ConfigurePage(page, pageAsRcb.VirtualPath);
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
                   ?? page.NormalizePath(virtualPath);

        if (!File.Exists(GetCodeFullPathForExistsCheck(path)))
            return !throwOnError
                ? null
                : throw l.Done(new FileNotFoundException("The file does not exist.", path));

        try
        {
            object? result = path.EndsWith(SourceCodeConstants.CsFileExtension)
                ? exCtx.GetDynamicApi().CreateInstance(path, npo, name: name, relativePath: null, throwOnError: throwOnError)
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
        var app = (overrideRootExCtx ?? exCtx /*ExCtxOrNull*/)?.GetTypedApi()?.AppTyped
                  ?? throw l.Done(new Exception("Absolute paths require an App, which was null"));
        var appFolder = app.Folder?.Path
                        ?? throw l.Done(new Exception("Absolute paths require the App folder, which was null"));
        return l.ReturnAndLog(Path.Combine(appFolder, path.TrimPrefixSlash()));
    }


    #endregion
}