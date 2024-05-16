using System.IO;
using System.Web;
using ToSic.Eav.Helpers;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;

#pragma warning disable CS0618

namespace ToSic.Sxc.Dnn;

internal class RazorCodeManager(RazorComponentBase parent, ILog parentLog) : HelperBase(parentLog, "Rzr.Code")
{
    public RazorComponentBase Parent = parent;

    /// <summary>
    /// The compiled code - or null
    /// </summary>
    private object _code;

    /// <summary>
    /// Determines if code has been compiled (or at least attempted)
    /// </summary>
    protected bool BuildComplete;

    /// <summary>
    /// Copy of any exception thrown when compiling the code
    /// </summary>
    protected Exception BuildException;

    /// <summary>
    ///  This tries to get the code and will show an exception if not ready. 
    /// </summary>
    public dynamic CodeOrException
    {
        get
        {
            TryToBuildCode();
            if (BuildException == null) return _code;
            throw ImproveExceptionMessage(BuildException);
        }
    }

    /// <summary>
    /// Internal accessor for the code, which does not throw exceptions but returns a null if not available
    /// </summary>
    internal dynamic CodeOrNull
    {
        get
        {
            TryToBuildCode();
            return _code;
        }
    }

    /// <summary>
    /// Try to build the code. If something fails, remember the exception in case we need it later.
    /// </summary>
    private bool TryToBuildCode()
    {
        var l = Log.Fn<bool>();
        if (BuildComplete) return l.Return(true);
        var codeFile = Parent.VirtualPath.Replace(".cshtml", ".code.cshtml").Backslash().AfterLast("\\");
        l.A($"Will try to load code from '{codeFile}");
        try
        {
            var compiled = Parent.RzrHlp.CreateInstance(codeFile);
            if (compiled != null && compiled is not RazorComponentCode)
                throw new(
                    $"Tried to compile the .Code file, but the type is '{compiled.GetType().Name}'. " +
                    $"Expected that it inherits from '{nameof(RazorComponentCode)}'. " +
                    "Please add '@inherits ToSic.Sxc.Dnn.RazorComponentCode' to the beginning of the 'xxx.code.cshtml' file. ");

            _code = compiled;
        }
        catch (Exception e)
        {
            BuildException = e;
        }

        BuildComplete = true;
        return l.Return(true, "code completed" + (BuildException == null ? "" : " with BuildExceptions"));
    }

    private static Exception ImproveExceptionMessage(Exception innerException)
    {
        switch (innerException)
        {
            case FileNotFoundException _:
                return new("Tried to compile matching .Code file - but couldn't find it. \n", innerException);
            case HttpCompileException _:
                return new("Error compiling .Code file. \n", innerException);
            default:
                return innerException;
        }
    }
}