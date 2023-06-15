using Microsoft.CSharp.RuntimeBinder;
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
            var help = FindHelp(ex);
            return help == null ? ex : new ExceptionWithHelp(help, ex);
        }

        internal CodeHelp FindHelp(Exception ex)
        {
            switch (ex)
            {
                // Check if we already wrapped it
                case ExceptionWithHelp _:
                    return null;
                case RuntimeBinderException _:
                    return CodeHelpList.FindHelp(ex, CodeHelpList.ListRuntime);
                case InvalidCastException _:
                    return CodeHelpList.FindHelp(ex, CodeHelpList.ListInvalidCast);
                case HttpCompileException _:
                    return CodeHelpList.FindHelp(ex, CodeHelpList.ListHttpCompile);
                default:
                    return null;
            }
        }
        
    }
}
