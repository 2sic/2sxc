using ToSic.Eav.Data.Processing;

namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Runs Copilot code generation after content-type schema changes.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CopilotContentTypeAutoGenerateAction(
    CopilotContentTypeAutoGenerateService autoGenerate)
    : ServiceBase(SxcLogName + ".AutoGen.CT", connect: [autoGenerate]),
        ILowCodeAction<ContentTypeChange, ContentTypeChange>
{
    public Task<ActionData<ContentTypeChange>> Run(LowCodeActionContext mainCtx, ActionData<ContentTypeChange> data)
    {
        var change = data.Data;
        var l = Log.Fn<ActionData<ContentTypeChange>>($"source:{change.Source}, app:{change.AppId}, typeId:{change.ContentTypeId}");

        var errors = data.Exceptions.ToList();
        errors.AddRange(autoGenerate.AutoGenerate(change));

        return Task.FromResult(l.Return(data with { Exceptions = errors }, $"errors:{errors.Count}"));
    }
}
