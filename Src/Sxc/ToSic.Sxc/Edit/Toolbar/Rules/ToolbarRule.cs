using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Edit.Internal.Toolbar;
using ToSic.Sxc.Web;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal abstract class ToolbarRule: ToolbarRuleBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="ui"></param>
    /// <param name="parameters"></param>
    /// <param name="operation"></param>
    /// <param name="operationCode">This is a string from the original command, which could affect the operator. It's only used to override the operator if there is a relevant match. </param>
    /// <param name="context"></param>
    protected ToolbarRule(string command, string ui = null, string parameters = null, char? operation = null, string operationCode = null, ToolbarContext context = null)
    {
        Command = command;
        Operation = operation == (char)OprNone ? null : operation; // reset operation if it's none
        Parameters = parameters ?? "";
        Ui = ui ?? "";
        Context = context;

        if (!operationCode.HasValue()) return;
        var targetCouldBeOperation = ToolbarRuleOperation.Pick(operationCode, OprUnknown);
        if (targetCouldBeOperation != (char)OprUnknown) 
            Operation = targetCouldBeOperation;
    }

    public char? Operation { get; protected set; }

    public string Command { get; protected set; }

    public string CommandValue { get; protected set; }

    public string Parameters { get; }

    public string Ui { get; }

    public virtual string GeneratedCommandParams() => UrlParts.ConnectParameters(Context.ToRuleString());

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
            
        result += "?" + UrlParts.ConnectParameters(genCmdParams, Parameters);
        return result;
    }

    protected string BuildValidParameterList(IEnumerable<(string, string)> values)
    {
        var keep = values.Where(set => !string.IsNullOrWhiteSpace(set.Item2));
        return string.Join("&", keep.Select(set => $"{set.Item1}={set.Item2}"));
    }
}