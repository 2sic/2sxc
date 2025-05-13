namespace ToSic.Sxc.Edit.Toolbar;

internal class ToolbarRuleCustom(
    string command,
    string ui = null,
    string parameters = null,
    char? operation = null,
    string operationCode = null,
    ToolbarContext context = null)
    : ToolbarRule(command, ui: ui, parameters: parameters, operation: operation, operationCode: operationCode,
        context: context);