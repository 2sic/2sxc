using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ToSic.Eav;
using ToSic.Eav.Code;
using ToSic.Eav.Plumbing;
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
        public Exception AddHelpIfKnownError(Exception ex, object mainCodeObject)
        {
            var help = FindHelp(ex);
            if (help != null) return new ExceptionWithHelp(help, ex);

            if (mainCodeObject is IHasCodeErrorHelp withHelp && withHelp.ErrorHelpers.SafeAny())
                help = FindHelp(ex, withHelp.ErrorHelpers);

            return help == null ? ex : new ExceptionWithHelp(help, ex);
        }

        internal CodeHelp FindHelp(Exception ex)
        {
            switch (ex)
            {
                // Check if we already wrapped it
                case ExceptionWithHelp _:
                    return null;
                case NamedArgumentException nae:
                    return new CodeHelp("named-parameters", null,
                        Parameters.HelpLink,
                        uiMessage: " ", detailsHtml: nae.Intro.Replace("\n", "<br>") + (nae.ParamNames.HasValue() ? $"<br>Param Names: <code>{nae.ParamNames}</code>": ""));
                case RuntimeBinderException _:
                    return FindHelp(ex, CodeHelpList.ListRuntime);
                case InvalidCastException _:
                    return FindHelp(ex, CodeHelpList.ListInvalidCast);
                case HttpCompileException _:
                    return FindHelp(ex, CodeHelpList.ListHttpCompile);
                default:
                    return null;
            }
        }

        public static CodeHelp FindHelp(Exception ex, List<CodeHelp> errorList)
        {
            var msg = ex?.Message;
            return msg == null ? null : errorList.FirstOrDefault(help => help.DetectRegex ? Regex.IsMatch(msg, help.Detect) : msg.Contains(help.Detect));
        }

    }
}
