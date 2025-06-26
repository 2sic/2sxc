namespace ToSic.Sxc.Oqt.Shared.Helpers;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ErrorHelper
{
    public static string ErrorMessage(Exception ex, bool isSuperUser = false)
    {
        var errorMessage = ex.Message;

        if (!isSuperUser) return errorMessage;

        errorMessage += " - " + ex.StackTrace;
        if (ex.InnerException == null) return errorMessage;

        errorMessage += " - " + ex.InnerException.Message;
        errorMessage += " - " + ex.InnerException.StackTrace;

        return errorMessage;
    }
}