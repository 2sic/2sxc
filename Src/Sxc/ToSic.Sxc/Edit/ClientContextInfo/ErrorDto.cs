using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ToSic.Eav.Obsolete;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Problems;
using static System.Text.Json.Serialization.JsonIgnoreCondition;
using static ToSic.Sxc.Blocks.Problems.ProblemReport;

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    public class ErrorDto
    {
        [JsonPropertyName("type")]
        public string Type { get; }

        [JsonPropertyName("problems")]
        [JsonIgnore(Condition = WhenWritingDefault)]
        public IEnumerable<ProblemReport> Problems { get; }

        internal ErrorDto(IBlock block, string errorCode, Exception exOrNull, CodeChangesInScope codeWarnings)
        {
            // New mechanism in 16.01
            Type = errorCode;

            if (!block.Context.User.IsSiteAdmin) return;

            // New problems report in 16.02
            var problems = new List<ProblemReport>(block.Problems);
            var additional = new ProblemSuggestions().AddSuggestions(block, exOrNull, errorCode);
            problems.AddRange(additional);

            var warnings = codeWarnings.List;
            if (warnings.SafeAny())
            {
                problems.Add(new ProblemReport()
                {
                    Link = "todo - warning",
                    Message = "This is old stuff",
                    Severity = ErrorSeverity.warning
                });
            }

            Problems = problems.Any() ? problems : null;
        }
    }
}
