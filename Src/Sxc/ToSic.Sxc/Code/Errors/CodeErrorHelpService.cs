using System;
#if NETFRAMEWORK
using HttpCompileException = System.Web.HttpCompileException;
#else
// TODO
using HttpCompileException = System.Exception;
#endif

namespace ToSic.Sxc.Code.Errors
{
    public class CodeErrorHelpService
    {
        public Exception AddHelpIfKnownError(Exception ex)
        {
            var additionalMsg = HelpText(ex);
            return additionalMsg == null 
                ? ex 
                : new ExceptionWithHelp(additionalMsg, ex);
        }

        public string HelpText(Exception ex)
        {
            // Check if we already wrapped it
            if (ex is ExceptionWithHelp) return null;

            if (ex is InvalidCastException)
                return CodeErrorsDatabase.FindAdditionalText(ex, CodeErrorsDatabase.InvalidCastExceptions);

            if (ex is HttpCompileException)
                return CodeErrorsDatabase.FindAdditionalText(ex, CodeErrorsDatabase.HttpCompileExceptions);

            return null;
        }
        
    }
}
