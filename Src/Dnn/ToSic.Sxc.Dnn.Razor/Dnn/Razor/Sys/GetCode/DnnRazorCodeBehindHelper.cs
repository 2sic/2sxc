using System.Web;
using ToSic.Razor.Blade;
#pragma warning disable CS0618 // Type or member is obsolete

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Helper to support the old (now deprecated) Code property, which was like a code-behind feature.
/// </summary>
/// <param name="parent"></param>
/// <param name="parentLog"></param>
internal class DnnRazorCodeBehindHelper(RazorComponentBase parent, ILog parentLog) : HelperBase(parentLog, "Rzr.Code")
{
    public RazorComponentBase Parent = parent;

    /// <summary>
    /// The compiled code - or null
    /// </summary>
    private object _code;

    /// <summary>
    /// Determines if code has been compiled (or at least attempted)
    /// </summary>
    private bool _buildComplete;

    /// <summary>
    /// Copy of any exception thrown when compiling the code
    /// </summary>
    private Exception _buildException;

    /// <summary>
    ///  This tries to get the code and will show an exception if not ready. 
    /// </summary>
    /// <param name="rzrGetCodeHlp"></param>
    public object GetCodeOrException(DnnRazorGetCodeHelper rzrGetCodeHlp)
    {
        TryToBuildCode(rzrGetCodeHlp);
        return _buildException == null
            ? _code
            : throw ImproveExceptionMessage(_buildException);
    }

    /// <summary>
    /// Try to build the code. If something fails, remember the exception in case we need it later.
    /// </summary>
    /// <param name="rzrGetCodeHlp"></param>
    private bool TryToBuildCode(DnnRazorGetCodeHelper rzrGetCodeHlp)
    {
        var l = Log.Fn<bool>();
        if (_buildComplete)
            return l.Return(true);
        var codeFile = Parent.VirtualPath
            .Replace(".cshtml", ".code.cshtml")
            .ToSystemPath()
            .AfterLast(Path.DirectorySeparatorChar.ToString());
        l.A($"Will try to load code from '{codeFile}");
        try
        {
            var compiled = rzrGetCodeHlp.CreateInstance(codeFile);
            if (compiled != null && compiled is not RazorComponentCode)
                throw new(
                    $"Tried to compile the .Code file, but the type is '{compiled.GetType().Name}'. " +
                    $"Expected that it inherits from '{nameof(RazorComponentCode)}'. " +
                    "Please add '@inherits ToSic.Sxc.Dnn.RazorComponentCode' to the beginning of the 'xxx.code.cshtml' file. ");

            _code = compiled;
        }
        catch (Exception e)
        {
            _buildException = e;
        }

        _buildComplete = true;
        return l.Return(true, "code completed" + (_buildException == null ? "" : " with BuildExceptions"));
    }

    private static Exception ImproveExceptionMessage(Exception innerException)
        => innerException switch
        {
            FileNotFoundException _ => new("Tried to compile matching .Code file - but couldn't find it. \n", innerException),
            HttpCompileException _ => new("Error compiling .Code file. \n", innerException),
            _ => innerException
        };
}