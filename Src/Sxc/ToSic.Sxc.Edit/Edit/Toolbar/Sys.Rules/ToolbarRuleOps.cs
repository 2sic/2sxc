namespace ToSic.Sxc.Edit.Toolbar.Sys.Rules;

internal enum ToolbarRuleOps
{
    OprAdd = ToolbarRuleOperation.AddOperation,
    OprAuto = ToolbarRuleOperation.AutoOperation,
    OprModify = ToolbarRuleOperation.ModifyOperation,
    OprRemove = ToolbarRuleOperation.RemoveOperation,
    [PrivateApi]
    OprUnknown = ToolbarRuleOperation.UnknownOperation,
    [PrivateApi]
    OprNone = ToolbarRuleOperation.NoOperation,
}