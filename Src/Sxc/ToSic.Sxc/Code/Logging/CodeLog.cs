using System.Runtime.CompilerServices;
using ToSic.Lib.Data;

// ReSharper disable ExplicitCallerInfoArgument

namespace ToSic.Sxc.Code;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CodeLog(ILog log) : Wrapper<ILog>(log ?? new Log(LogConstants.NameUnknown)), ICodeLog
{
    /// <inheritdoc />
    public string Add(string message, [CallerFilePath] string cPath = null, [CallerMemberName] string cName = null, [CallerLineNumber] int cLine = 0)
    {
        GetContents().A(message, cPath, cName, cLine);
        return message;
    }

    /// <inheritdoc />
    public void Warn(string message, [CallerFilePath] string cPath = null, [CallerMemberName] string cName = null, [CallerLineNumber] int cLine = 0) 
        => GetContents().W(message, cPath, cName, cLine);

    public void Exception(Exception ex, [CallerFilePath] string cPath = null, [CallerMemberName] string cName = null, [CallerLineNumber] int cLine = 0)
        => GetContents().Ex(ex, cPath, cName, cLine);


    /// <inheritdoc />
    public Action<string> Call(string parameters = null, string message = null, bool useTimer = false,
        [CallerFilePath] string cPath = null, [CallerMemberName] string cName = null, [CallerLineNumber] int cLine = 0)
    {
        // must call the opener first, then return the closing function
        var call = GetContents().Fn(parameters, message, timer: useTimer, cPath: cPath, cName: cName, cLine: cLine);
        return finalMsg => call.Done(finalMsg);
    }

    /// <inheritdoc />
    public Func<T, string, T> Call<T>(string parameters = null, string message = null, bool useTimer = false,
        [CallerFilePath] string cPath = null, [CallerMemberName] string cName = null, [CallerLineNumber] int cLine = 0)
    {
        // must call the opener first, then return the closing function
        var call = GetContents().Fn<T>(parameters, message, timer: useTimer, cPath: cPath, cName: cName, cLine: cLine);
        return (data, finalMsg) => call.Return(data, finalMsg);
    }

    /// <inheritdoc />
    public bool Preserve
    {
        get => (GetContents() as Log)?.Preserve ?? false; // default to false if there is no log
        set { if (GetContents() is Log log1) log1.Preserve = value; }
    }
}