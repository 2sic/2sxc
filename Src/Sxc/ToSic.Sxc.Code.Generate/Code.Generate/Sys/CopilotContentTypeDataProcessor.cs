using ToSic.Eav.Data.Processing;
using static ToSic.Eav.Data.Processing.DataProcessingEvents;
using static ToSic.Eav.Data.Processing.DataProcessingContextSources;

namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Runs copilot code generation after content-type schema saves.
/// Uses the shared <see cref="PostSave"/> action plus schema context.
/// For schema-triggered runs the context is authoritative and the entity payload may be null.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CopilotContentTypeDataProcessor(
    CopilotContentTypeAutoGenerateService autoGenerate)
    : ServiceBase(SxcLogName + ".AutoGen.CT", connect: [autoGenerate]), IDataProcessor
{
    /// <summary>
    /// Handle post-save processing for a changed content-type and run matching auto-generate configurations.
    /// </summary>
    /// <param name="action">Expected to be <see cref="PostSave"/> for schema triggers.</param>
    /// <param name="data">Processor payload containing the schema trigger context.</param>
    /// <returns>Original or enriched processor result with collected exceptions.</returns>
    public async Task<DataProcessorResult<IEntity?>> Process(string action, DataProcessorResult<IEntity?> data)
    {
        var l = Log.Fn<DataProcessorResult<IEntity?>>($"action:{action}");

        var context = data.Context;
        if (!IsContentTypeSchemaPostSave(action, context))
            return l.Return(data, "unsupported action/context");

        var errors = data.Exceptions.ToList();
        var appId = context!.AppId;
        if (!appId.HasValue)
        {
            Log.A("Copilot auto-generate skipped: schema context is missing AppId.");
            return l.Return(data, "missing app id");
        }

        var changedTypeNameId = context.ContentTypeNameId;
        if (changedTypeNameId.IsEmptyOrWs())
        {
            Log.A("Copilot auto-generate skipped: schema context is missing content-type name-id.");
            return l.Return(data, "missing content-type name-id");
        }

        var source = context.Source ?? "";
        Log.A($"Copilot auto-generate: source '{source}' for content-type '{changedTypeNameId}'.");
        // The IDataProcessor remains the external contract.
        // Internally we now map the validated processor envelope to a typed ILowCodeAction pipeline
        // so schema context and upstream processor state stay intact through the pipeline.
        var triggerData = new ActionData<CopilotSchemaTrigger>(new(
            AppId: appId.Value,
            ContentTypeNameId: changedTypeNameId,
            Source: source))
        {
            Context = data.Context,
            Exceptions = errors,
            Decision = data.Decision,
            LogMessage = data.LogMessage,
            ErrorMessage = data.ErrorMessage,
        };

        var pipelineResult = await autoGenerate.Run(triggerData);

        var result = ResultWithPipelineState(data, pipelineResult);
        return l.Return(result, $"errors: {result.Exceptions.Count}");
    }

    private static DataProcessorResult<IEntity?> ResultWithPipelineState(
        DataProcessorResult<IEntity?> original,
        ActionData<CopilotGenerationBatch> pipelineResult)
        => original with
        {
            Exceptions = pipelineResult.Exceptions,
            Decision = pipelineResult.Decision,
            LogMessage = pipelineResult.LogMessage,
            ErrorMessage = pipelineResult.ErrorMessage,
        };

    private static bool IsContentTypeSchemaPostSave(string action, DataProcessingContext? context)
        => action.EqualsInsensitive(PostSave)
           && context?.Source is { } source
           && (source.EqualsInsensitive(ContentType) || source.EqualsInsensitive(ContentTypeField));
}
