﻿#if NETFRAMEWORK
using HttpCompileException = System.Web.HttpCompileException;
#else
// TODO: @STV What's the real compile exception type? we need it, so that the errors are better
using HttpCompileException = System.Exception;
#endif
using System.Text.RegularExpressions;
using Microsoft.CSharp.RuntimeBinder;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sys.Code.Help;
using ToSic.Sys.Exceptions;

namespace ToSic.Sxc.Code.Sys.CodeErrorHelp;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class CodeErrorHelpService: ServiceBase
{
    public CodeErrorHelpService() : base("Sxc.CErrHS")
    {
        Log.A("Trying to add help to error, something must have happened");
    }

    public Exception AddHelpForCompileProblems(Exception ex, CodeFileInfo fileInfo)
    {
        var l = Log.Fn<Exception>();
        try
        {
            // Check if it already has help included
            if (ex is IExceptionWithHelp) 
                return l.Return(ex, "already has help");

            if (!fileInfo.Help.SafeAny())// !CodeHelpDb.CompileHelp.TryGetValue(fileInfo, out var list))
                return l.Return(ex, "no additional help found");

            var help = FindManyOrNull(ex, fileInfo.Help);
            return help == null 
                ? l.Return(ex)
                : l.Return(new ExceptionWithHelp(help, ex), "added help");
        }
        catch (Exception myEx)
        {
            Log.Ex("Something went wrong, inner error", myEx);
            return l.Return(ex, "just return original exception");
        }
    }

    public Exception AddHelpIfKnownError(Exception ex, object? mainCodeObject)
    {
        var l = Log.Fn<Exception>();
        try
        {
            // Check if it already has help included
            if (ex is IExceptionWithHelp)
                return l.Return(ex, "already has help");

            var help = FindHelp(ex);
            if (help != null)
                return l.Return(new ExceptionWithHelp(help, ex), "added help");

            if (mainCodeObject is IHasCodeHelp withHelp && withHelp.ErrorHelpers.SafeAny())
                help = FindHelp(ex, withHelp.ErrorHelpers);

            return help == null
                ? l.Return(ex)
                : l.Return(new ExceptionWithHelp(help, ex), "added help");
        }
        catch (Exception myEx)
        {
            Log.Ex("Something went wrong, inner error", myEx);
            return l.Return(ex, "just return original exception");
        }

    }

    internal CodeHelp? FindHelp(Exception ex)
    {
        switch (ex)
        {
            // Check if we already wrapped it
            case ExceptionWithHelp _:
                return null;
            //case NamedArgumentException nae:
            //    return new CodeHelp("named-parameters", null,
            //        Parameters.HelpLink,
            //        uiMessage: " ", detailsHtml: nae.Intro.Replace("\n", "<br>") + (nae.ParamNames.HasValue() ? $"<br>Param Names: <code>{nae.ParamNames}</code>": ""));
            case RuntimeBinderException _:
                return FindHelp(ex, HelpForCommonProblems.HelpForRuntimeProblems);
            case InvalidCastException _:
                return FindHelp(ex, HelpForCommonProblems.HelpForInvalidCast);
            case HttpCompileException _:
                return FindHelp(ex, HelpForCommonProblems.HelpForHttpCompile);
            default:
                return null;
        }
    }

    private static CodeHelp? FindHelp(Exception? ex, List<CodeHelp> errorList)
    {
        var msg = $"{ex?.Message}{ex?.StackTrace}";
        return msg.IsEmpty()
            ? null
            : errorList.FirstOrDefault(help
                => help.Detect.HasValue()
                   && (help.DetectRegex
                       ? Regex.IsMatch(msg, help.Detect)
                       : msg.Contains(help.Detect ?? "@#423-dummy-should-never-find"))
            );
    }
    private static List<CodeHelp>? FindManyOrNull(Exception? ex, List<CodeHelp> errorList)
    {
        var msg = ex?.Message;
        if (msg.IsEmptyOrWs())
            return null;
        var list = errorList
            .Where(help =>
                help.Detect.HasValue()
                && (help.DetectRegex ? Regex.IsMatch(msg, help.Detect) : msg.Contains(help.Detect))
            )
            .ToList();
        return list.Any() ? list : null;
    }

}