using System.IO;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Code.Internal.CodeRunHelpers;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class RazorHelperBase(string logName) : CodeHelperBase(logName)
{
    public List<Exception> ExceptionsOrNull { get; private set; }

    public void Add(Exception ex) => (ExceptionsOrNull ??= []).Add(ex);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="overrideCodeRoot">Insert another code Root, ATM a patch for Oqtane Razor</param>
    /// <returns></returns>
    protected string ResolvePathIfAbsoluteToApp(string path, ICodeApiService overrideCodeRoot = default)
    {
        var l = Log.Fn<string>(path);
        if (path == null || (!path.StartsWith("/") && !path.StartsWith("\\")))
            return l.ReturnNull("not absolute, return null");

        l.A("Will try to use absolute path relative to the app.");

        if (!path.EndsWith(CodeCompiler.CsFileExtension))
            throw l.Done(new ArgumentException("Only '.cs' file paths can start with a slash"));
        var app = (overrideCodeRoot ?? _CodeApiSvc)?.AppTyped
                  ?? throw l.Done(new Exception("Absolute paths require an App, which was null"));
        var appFolder = app.Folder?.Path
                        ?? throw l.Done(new Exception("Absolute paths require the App folder, which was null"));
        return l.ReturnAndLog(Path.Combine(appFolder, path.TrimPrefixSlash()));
    }

    #region CreateInstance / GetCode

    public object GetCode(string path, NoParamOrder noParamOrder = default, string className = default) 
        => GetCode(path, noParamOrder: noParamOrder, name: className, throwOnError: true);

    /// <summary>
    /// Creates instances of the shared pages with the given relative path
    /// </summary>
    /// <returns></returns>
    private object GetCode(string virtualPath,
        NoParamOrder noParamOrder,
        string name,
        bool throwOnError)
    {
        // Note: Don't do parameter checks, as they have already been done
        // and the warnings are a bit different depending on the public signature

        var l = Log.Fn<object>($"'{virtualPath}', '{name}'");

        if (virtualPath.IsEmptyOrWs())
            return !throwOnError
                ? null as object
                : throw l.Done(new ArgumentException("path can't be empty"));

        var path = ResolvePathIfAbsoluteToApp(virtualPath)?.ForwardSlash().PrefixSlash()
                   ?? GetCodeNormalizePath(virtualPath);

        if (!File.Exists(GetCodeFullPathForExistsCheck(path)))
            return !throwOnError
                ? null as object
                : throw l.Done(new FileNotFoundException("The file does not exist.", path));

        try
        {
            object result = path.EndsWith(CodeCompiler.CsFileExtension)
                ? _CodeApiSvc.CreateInstance(path, noParamOrder, name: name, relativePath: null, throwOnError: throwOnError)
                : GetCodeCshtml(path);
            return l.Return(result, "ok");
        }
        catch (Exception ex)
        {
            l.Done(ex);
            if (throwOnError) throw;
            return null;
        }
    }


    public object CreateInstance(string virtualPath,
        NoParamOrder noParamOrder = default,
        string name = null,
        string relativePath = null,
        bool throwOnError = true
    ) => GetCode(virtualPath: virtualPath, noParamOrder: noParamOrder, name: name, throwOnError: throwOnError);

    protected abstract object GetCodeCshtml(string path);

    #endregion

    protected abstract string GetCodeFullPathForExistsCheck(string path);
    protected abstract string GetCodeNormalizePath(string virtualPath);
}