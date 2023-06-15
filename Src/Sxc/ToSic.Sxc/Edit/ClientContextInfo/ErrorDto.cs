using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Problems;
using ToSic.Sxc.Code.Errors;
using static System.Text.Json.Serialization.JsonIgnoreCondition;

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    public class ErrorDto
    {
        [JsonPropertyName("type")]
        public string Type { get; }

        [JsonPropertyName("problems")]
        [JsonIgnore(Condition = WhenWritingDefault)]
        public IEnumerable<ProblemReport> Problems { get; }

        internal ErrorDto(IBlock block, string errorCode, Exception exOrNull)
        {
            // New mechanism in 16.01
            Type = errorCode;

            if (!block.Context.User.IsSiteAdmin) return;

            // New problems report in 16.02
            var problems = new List<ProblemReport>(block.Problems);
            var additional = new ProblemSuggestions().AddSuggestions(block, exOrNull, errorCode);
            problems.AddRange(additional);

            Problems = problems.Any() ? problems : null;
        }
    }
}
