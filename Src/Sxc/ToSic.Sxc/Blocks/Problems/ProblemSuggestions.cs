using System;
using System.Collections.Generic;

namespace ToSic.Sxc.Blocks.Problems
{
    internal class ProblemSuggestions
    {
        public IEnumerable<ProblemReport> AddSuggestions(IBlock block, string errorCode)
        {
            if (errorCode == null || block?.App == null) return Array.Empty<ProblemReport>();

            var suggestions = new List<ProblemReport>();
            suggestions.Add(new ProblemReport
            {
                Scope = ProblemReport.ErrorScope.view,
                Severity = ProblemReport.ErrorSeverity.info,
                Message = "test",
                Link = "https://go.2sxc.org/test"
            });

            // Special suggestion for Blog v 6
            var app = block.App;
            if (app.NameId == "72e406fd-500f-4632-82ca-942b22358b56" && app.Configuration.Version.CompareTo(new Version(6, 0, 1)) == 0)
            {
                suggestions.Add(new ProblemReport
                {
                    Scope = ProblemReport.ErrorScope.app,
                    Severity = ProblemReport.ErrorSeverity.warning,
                    Message = "See todo",
                    Link = "https://go.2sxc.org/todo-blog6"
                });
            }
            return suggestions;
        }
    }
}
