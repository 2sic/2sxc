using ToSic.Eav.Data.Processing;
using static ToSic.Eav.Data.Processing.DataProcessingEvents;
using static ToSic.Eav.Data.Processing.DataProcessingContextSources;

namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Runs copilot code generation after content-type schema saves.
/// Uses the shared <see cref="PostSave"/> action plus schema context and expects an
/// <see cref="IEntity"/> payload carrying the app id and changed content-type.
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
    /// <param name="data">Processor payload containing the trigger entity context.</param>
    /// <returns>Original or enriched processor result with collected exceptions.</returns>
    public Task<DataProcessorResult<IEntity?>> Process(string action, DataProcessorResult<IEntity?> data)
    {
        var l = Log.Fn<DataProcessorResult<IEntity?>>($"action:{action}");

        var context = data.Context;
        if (!IsContentTypeSchemaPostSave(action, context))
            return Task.FromResult(l.Return(data, "unsupported action/context"));

        var errors = data.Exceptions.ToList();
        var triggerEntity = data.Data;
        if (triggerEntity == null)
        {
            Log.A("Copilot auto-generate skipped: missing IEntity context for content-type post-save.");
            return Task.FromResult(l.Return(data, "missing entity context"));
        }

        var appId = triggerEntity.AppId;
        var changedTypeNameId = triggerEntity.Type.NameId;
        if (changedTypeNameId.IsEmptyOrWs())
        {
            Log.A("Copilot auto-generate skipped: entity content-type name-id is empty.");
            return Task.FromResult(l.Return(data, "missing content-type name-id"));
        }

        var source = context!.Source ?? "";
        Log.A($"Copilot auto-generate: source '{source}' for content-type '{changedTypeNameId}'.");
        errors.AddRange(autoGenerate.Generate(appId, changedTypeNameId, origin: source));

        var result = ResultWithErrors(data, errors);
        return Task.FromResult(l.Return(result, $"errors: {result.Exceptions.Count}"));
    }

    private static DataProcessorResult<IEntity?> ResultWithErrors(DataProcessorResult<IEntity?> original, List<Exception> errors)
        => errors.Count == original.Exceptions.Count
            ? original
            : original with { Exceptions = errors };

    private static bool IsContentTypeSchemaPostSave(string action, DataProcessingContext? context)
        => action.EqualsInsensitive(PostSave)
           && context?.Source is { } source
           && (source.EqualsInsensitive(ContentType) || source.EqualsInsensitive(ContentTypeField));
}
