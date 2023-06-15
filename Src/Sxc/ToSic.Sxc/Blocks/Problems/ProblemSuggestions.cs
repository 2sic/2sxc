using System;
using System.Collections.Generic;
using ToSic.Sxc.Code.Errors;
// ReSharper disable ConvertTypeCheckPatternToNullCheck

namespace ToSic.Sxc.Blocks.Problems
{
    internal class ProblemSuggestions
    {
        public IEnumerable<ProblemReport> AddSuggestions(IBlock block, Exception exOrNull, string errorCode)
        {
            var suggestions = new List<ProblemReport>();

            // Add suggestions for any exceptions in the code
            if ((exOrNull as IExceptionWithHelp)?.Help is CodeError help)
            {
                var problem = new ProblemReport
                {
                    Link = help.Link,
                    Message = help.UiMessage,
                    Severity = ProblemReport.ErrorSeverity.warning,
                };
                suggestions.Add(problem);
            }


            if (errorCode == null || block?.App == null) return suggestions;

            //suggestions.Add(new ProblemReport
            //{
            //    Scope = ProblemReport.ErrorScope.view,
            //    Severity = ProblemReport.ErrorSeverity.info,
            //    Message = "test",
            //    Link = "https://go.2sxc.org/test"
            //});

            // Special suggestion for Blog v 6
            var app = block.App;
            if (app.NameId == "72e406fd-500f-4632-82ca-942b22358b56" && app.Configuration.Version.CompareTo(new Version(6, 0, 1)) == 0)
            {
                suggestions.Add(new ProblemReport
                {
                    Scope = ProblemReport.ErrorScope.app,
                    Severity = ProblemReport.ErrorSeverity.error,
                    Message = "The Blog App 6.0.1 used some very new APIs in 2sxc 16.01 which had to be revised in 16.02. Check this guide for what to fix.",
                    Link = "https://go.2sxc.org/app-upgrade-blog601"
                });
            }
            return suggestions;
        }
    }
}
