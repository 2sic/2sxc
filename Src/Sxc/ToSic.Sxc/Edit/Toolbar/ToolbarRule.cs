using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Edit.Toolbar
{
    public abstract class ToolbarRule: ToolbarRuleBase
    {
        protected ToolbarRule(string command, string ui = null, string parameters = null, char? operation = null)
        {
            Command = command;
            Operation = operation;
            Parameters = parameters ?? "";
            Ui = ui ?? "";
        }

        public char? Operation { get; }

        public string Command { get; }

        public string CommandValue { get; protected set; }

        public string Parameters { get; }

        public string Ui { get; }

        public virtual string GeneratedCommandParams() => "";

        public virtual string GeneratedUiParams() => "";

        public override string ToString()
        {
            var result = Operation + Command + (CommandValue.HasValue() ? "=" + CommandValue : "");

            var genUi = GeneratedUiParams();
            if (genUi.HasValue()) result += "&" + genUi;
            if (Ui.HasValue()) result += "&" + Ui;
            
            var genCmdParams = GeneratedCommandParams();
            var hasGeneratedCmdParams = genCmdParams.HasValue();
            var hasCmdParams = Parameters.HasValue();

            // Stop if nothing to add
            if (!hasGeneratedCmdParams && !hasCmdParams) return result;
            
            result += "?";
            if (hasGeneratedCmdParams) result += genCmdParams;
            if (hasGeneratedCmdParams && hasCmdParams) result += "&";
            if (hasCmdParams) result += Parameters;
            return result;
        }

        protected string BuildValidParameterList(IEnumerable<(string, string)> values)
        {
            var keep = values.Where(set => !string.IsNullOrWhiteSpace(set.Item2));
            return string.Join("&", keep.Select(set => $"{set.Item1}={set.Item2}"));
        }
    }
}
