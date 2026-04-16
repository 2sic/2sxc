using ToSic.Eav.Data.Processing;

namespace ToSic.Sxc.Code.Generate.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CopilotContentTypeAutoGenerateService(
    CopilotSchemaResolveBatchAction resolveBatch,
    CopilotSchemaExecuteBatchAction executeBatch)
    : ServiceBase(SxcLogName + ".AutoGen.Run", connect: [resolveBatch, executeBatch])
{
    /// <summary>
    /// Orchestrate the internal low-code pipeline for copilot schema generation.
    /// The outer trigger remains <see cref="IDataProcessor"/>; low-code actions only
    /// structure the work inside this assembly.
    /// </summary>
    public async Task<ActionData<CopilotGenerationBatch>> Run(ActionData<CopilotSchemaTrigger> triggerData)
    {
        var trigger = triggerData.Data;
        var l = Log.Fn<ActionData<CopilotGenerationBatch>>(
            $"origin:{trigger.Source}, app:{trigger.AppId}, type:{trigger.ContentTypeNameId}");
        var lowCodeContext = new LowCodeActionContext();

        var resolved = await resolveBatch.Run(lowCodeContext, triggerData);

        var executed = await executeBatch.Run(lowCodeContext, resolved);

        return l.Return(executed, $"jobs:{executed.Data.Jobs.Count}, errors:{executed.Exceptions.Count}");
    }
}
