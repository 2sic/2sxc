using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Code.Help;
using ToSic.Eav.Plumbing;

// ReSharper disable ConvertTypeCheckPatternToNullCheck

namespace ToSic.Sxc.Blocks.Problems
{
    internal class ProblemSuggestions
    {
        public IEnumerable<ProblemReport> AddSuggestions(IBlock block, List<Exception> exsOrNull, string errorCode)
        {
            var suggestions = new List<ProblemReport>();

            // Add suggestions for any exceptions in the code
            if (exsOrNull.SafeAny())
            {
                // deduplicate - in case we have many of the same errors
                var unique = exsOrNull
                    .GroupBy(e => e.Message)
                    .Select(g => g.First())
                    .ToList();

                foreach (var ex in unique)
                    if ((ex as IExceptionWithHelp)?.Helps is List<CodeHelp> helps)
                        helps.ForEach(h => suggestions.Add(new ProblemReport
                        {
                            Link = h.Link.NullIfNoValue(),
                            Message = h.DetailsHtml ?? h.UiMessage,
                            Severity = ProblemReport.ErrorSeverity.warning,
                        }));
            }


            if (errorCode == null || block?.App == null) return suggestions;

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
