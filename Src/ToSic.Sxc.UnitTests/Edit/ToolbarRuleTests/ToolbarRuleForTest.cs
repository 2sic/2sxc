using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Tests.EditTests.ToolbarRuleTests;

/// <summary>
/// Just a wrapper around toolbar rules, so we can test it, as the constructor is protected.
/// </summary>
/// <param name="command"></param>
/// <param name="ui"></param>
/// <param name="parameters"></param>
/// <param name="operation"></param>
/// <param name="operationCode"></param>
/// <param name="context"></param>
internal class ToolbarRuleForTest(
    string command,
    string ui = null,
    string parameters = null,
    char? operation = null,
    string operationCode = null,
    ToolbarContext context = null)
    : ToolbarRule(command, ui, parameters, operation, operationCode, context);