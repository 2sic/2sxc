using ToSic.Eav.Apps;
using ToSic.Eav.Data.Processing;

namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Resolve a schema trigger into concrete generation jobs.
/// This keeps filtering and spec-building separate from file generation side effects.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CopilotSchemaResolveBatchAction(
    IAppReaderFactory appReaders)
    : ServiceBase(SxcLogName + ".AutoGen.Resolve", connect: [appReaders]),
        ILowCodeAction<CopilotSchemaTrigger, CopilotGenerationBatch>
{
    private const string DataCopilotConfigurationContentType = "DataCopilotConfiguration";
    private const string FieldAutoGenerate = "AutoGenerate";
    private const string FieldCodeGenerator = "CodeGenerator";
    private const string FieldNamespace = "Namespace";
    private const string FieldTargetFolder = "TargetFolder";
    private const string FieldContentTypes = "ContentTypes";
    private const string FieldPrefix = "Prefix";
    private const string FieldSuffix = "Suffix";
    private const string FieldEdition = "Edition";

    public Task<ActionData<CopilotGenerationBatch>> Run(LowCodeActionContext mainCtx, ActionData<CopilotSchemaTrigger> result)
    {
        var trigger = result.Data;
        var l = Log.Fn<ActionData<CopilotGenerationBatch>>(
            $"origin:{trigger.Source}, app:{trigger.AppId}, type:{trigger.ContentTypeNameId}");
        var errors = result.Exceptions.ToList();

        if (trigger.ContentTypeNameId.IsEmptyOrWs())
        {
            Log.A("Copilot auto-generate skipped: content-type name-id is empty.");
            return Task.FromResult(l.Return(Output(result, EmptyBatch(trigger), errors), "missing content-type name-id"));
        }

        var appReader = appReaders.Get(trigger.AppId);
        var changedType = appReader.TryGetContentType(trigger.ContentTypeNameId);
        if (changedType == null)
        {
            errors.Add(new ArgumentException(
                $"Content-Type '{trigger.ContentTypeNameId}' not found in app '{trigger.AppId}'."));
            Log.A($"Copilot auto-generate skipped: content-type '{trigger.ContentTypeNameId}' not found in app '{trigger.AppId}'.");
            return Task.FromResult(l.Return(Output(result, EmptyBatch(trigger), errors), "content-type not found"));
        }

        var jobs = appReader.List
            .GetAll(DataCopilotConfigurationContentType)
            .Where(configuration => configuration.Get<bool>(FieldAutoGenerate))
            .Select(configuration => BuildJob(configuration, changedType))
            .Where(job => job != null)
            .Cast<CopilotGenerationJob>()
            .ToList();

        if (!jobs.Any())
            Log.A($"Copilot auto-generate: no matching configurations for content-type '{changedType.NameId}' ({trigger.Source}).");

        var batch = new CopilotGenerationBatch(trigger, changedType, jobs);
        return Task.FromResult(l.Return(Output(result, batch, errors), $"jobs:{jobs.Count}"));
    }

    private static CopilotGenerationBatch EmptyBatch(CopilotSchemaTrigger trigger)
        => new(trigger, ChangedType: null, Jobs: []);

    private static ActionData<CopilotGenerationBatch> Output(
        ActionData<CopilotSchemaTrigger> source,
        CopilotGenerationBatch batch,
        List<Exception> errors)
        => new ActionData<CopilotGenerationBatch>(batch)
        {
            Context = source.Context,
            Exceptions = errors,
            Decision = source.Decision,
            LogMessage = source.LogMessage,
            ErrorMessage = source.ErrorMessage,
        };

    private static CopilotGenerationJob? BuildJob(IEntity configuration, IContentType changedType)
    {
        var generatorName = Sanitize(configuration.Get<string>(FieldCodeGenerator));
        if (generatorName.IsEmptyOrWs())
            return null;

        var selectedTypes = Normalize(configuration.Get<string>(FieldContentTypes));
        if (selectedTypes != null && !selectedTypes.Any(selection =>
                selection.EqualsInsensitive(changedType.NameId) || selection.EqualsInsensitive(changedType.Name)))
            return null;

        var specs = new FileGeneratorSpecs
        {
            AppId = changedType.AppId,
            Configuration = $"{configuration.EntityId} {configuration.GetBestTitle()}",
            Namespace = Sanitize(configuration.Get<string>(FieldNamespace)),
            TargetPath = Sanitize(configuration.Get<string>(FieldTargetFolder)),
            // Preserve current semantics: empty config means generate the normal/full scope,
            // explicit config means generate only the selected subset.
            ContentTypes = selectedTypes,
            Prefix = Sanitize(configuration.Get<string>(FieldPrefix)),
            Suffix = Sanitize(configuration.Get<string>(FieldSuffix)),
            Edition = Sanitize(configuration.Get<string>(FieldEdition)),
        };

        return new(configuration.EntityId, generatorName, specs);
    }

    private static string? Sanitize(string? value)
        => value.HasValue() ? value.Trim() : null;

    private static ICollection<string>? Normalize(string? raw)
    {
        var cleaned = Sanitize(raw);
        return cleaned == null
            ? null
            : Normalize([cleaned]);
    }

    private static ICollection<string>? Normalize(IEnumerable<string>? raw)
    {
        if (raw == null)
            return null;

        var cleaned = raw
            .SelectMany(item => item?
                .Split([',', ';', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
                ?? [])
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return cleaned.Any()
            ? cleaned
            : null;
    }
}
