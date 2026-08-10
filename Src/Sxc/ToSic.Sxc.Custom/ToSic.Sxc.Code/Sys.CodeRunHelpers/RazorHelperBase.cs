namespace ToSic.Sxc.Code.Sys.CodeRunHelpers;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class RazorHelperBase(string logName) : CodeHelperBase(logName)
{
    public List<Exception>? ExceptionsOrNull { get; private set; }

    public Exception Add(Exception ex)
    {
        (ExceptionsOrNull ??= []).Add(ex);
        return ex;
    }


    // #DropOqtaneGetCodeV22
    
    //#region CreateInstance / GetCode

    //protected abstract object GetCodeCshtml(string path);

    //#endregion

    //protected abstract string GetCodeFullPathForExistsCheck(string path);
    //protected abstract string GetCodeNormalizePath(string virtualPath);
}