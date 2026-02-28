using ToSic.Eav.Apps;
using ToSic.Eav.Data.Processing;
using ToSic.Sys.DI;
using static ToSic.Eav.Data.Processing.DataProcessingEvents;

namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Runs copilot code generation after content-type schema saves.
/// Uses content-type specific post-save actions and expects an <see cref="IEntity"/>
/// context carrying the app id and changed content-type.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CopilotContentTypeDataProcessor(
    FileSaver fileSaver,
    LazySvc<IEnumerable<IFileGenerator>> generators,
    IAppReaderFactory appReaders)
    : ServiceBase(SxcLogName + ".AutoGen.CT", connect: [fileSaver, generators, appReaders]), IDataProcessor
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

    /// <summary>
    /// Handle post-save processing for a changed content-type and run matching auto-generate configurations.
    /// </summary>
    /// <param name="action">Expected to be one of the content-type post-save actions.</param>
    /// <param name="data">Processor payload containing the trigger entity context.</param>
    /// <returns>Original or enriched processor result with collected exceptions.</returns>
    public Task<DataProcessorResult<IEntity?>> Process(string action, DataProcessorResult<IEntity?> data)
    {
        var l = Log.Fn<DataProcessorResult<IEntity?>>($"action:{action}");
        // 1) Guard: this processor only reacts to content-type schema post-save events.
        if (!IsContentTypeSchemaAction(action))
            return Task.FromResult(l.Return(data, "unsupported action"));

        // 2) Keep processor-chain diagnostics and resolve changed content-type from trigger entity.
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

        var appReader = appReaders.Get(appId);
        var changedType = appReader.TryGetContentType(changedTypeNameId);
        if (changedType == null)
        {
            errors.Add(new ArgumentException(
                $"Content-Type '{changedTypeNameId}' not found in app '{appId}'."));
            Log.A($"Copilot auto-generate skipped: content-type '{changedTypeNameId}' not found in app '{appId}'.");
            return Task.FromResult(l.Return(ResultWithErrors(data, errors), "content-type not found"));
        }

        // 3) Collect all auto-generate configurations that target this content-type.
        var matchingConfigurations = appReader.List
            .GetAll(DataCopilotConfigurationContentType)
            // AutoGenerate is the explicit opt-in on each copilot configuration.
            .Where(configuration => configuration.Get<bool>(FieldAutoGenerate))
            .Select(configuration => BuildRunConfiguration(configuration, changedType))
            .Where(configuration => configuration != null)
            .Cast<RunConfiguration>()
            .ToList();

        if (!matchingConfigurations.Any())
        {
            Log.A($"Copilot auto-generate: no matching configurations for content-type '{changedType.NameId}'.");
            return Task.FromResult(l.Return(data, "no matching auto-generate configurations"));
        }

        // 4) Execute each matching generation job; continue even if one fails.
        foreach (var configuration in matchingConfigurations)
        {
            try
            {
                var generator = generators.Value.FirstOrDefault(g => g.Name.EqualsInsensitive(configuration.GeneratorName));
                if (generator == null)
                {
                    Log.A($"Copilot auto-generate: generator '{configuration.GeneratorName}' not found.");
                    errors.Add(new InvalidOperationException(
                        $"Generator '{configuration.GeneratorName}' not found for configuration '{configuration.ConfigurationId}'."));
                    // Continue so other valid configurations can still generate their outputs.
                    continue;
                }

                (generator as IHasLog)?.LinkLog(Log);
                // FileSaver encapsulates full generate + persist semantics for each generator implementation.
                fileSaver.GenerateAndSaveFiles(generator, configuration.Specs);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                Log.Ex(ex);
            }
        }

        // 5) Return aggregated result (including any generation errors).
        var result = ResultWithErrors(data, errors);
        return Task.FromResult(l.Return(result, $"processed {matchingConfigurations.Count} configuration(s)"));
    }

    private static RunConfiguration? BuildRunConfiguration(IEntity configuration, IContentType changedType)
    {
        var generatorName = Sanitize(configuration.Get<string>(FieldCodeGenerator));
        // TODO: Empty generator name means "use default"
        if (generatorName.IsEmptyOrWs())
            return null;

        var selectedTypes = Normalize(configuration.Get<string>(FieldContentTypes));
        // Empty ContentTypes means "all", otherwise match by NameId/Name and skip non-target configs.
        if (selectedTypes != null && !selectedTypes.Any(selection =>
                selection.EqualsInsensitive(changedType.NameId) || selection.EqualsInsensitive(changedType.Name)))
            return null;

        var specs = new FileGeneratorSpecs
        {
            AppId = changedType.AppId,
            Configuration = $"{configuration.EntityId} {configuration.GetBestTitle()}",
            Namespace = Sanitize(configuration.Get<string>(FieldNamespace)),
            TargetPath = Sanitize(configuration.Get<string>(FieldTargetFolder)),
            // Keep generation scope aligned with configuration semantics:
            // - null => generate default/full set (same behavior as manual generate)
            // - explicit list => generate that selected subset.
            // This preserves cross-type strong typing on entity fields (e.g. Author/Category),
            // which can degrade to ITypedItem if generation is restricted to one type only.
            ContentTypes = selectedTypes,
            Prefix = Sanitize(configuration.Get<string>(FieldPrefix)),
            Suffix = Sanitize(configuration.Get<string>(FieldSuffix)),
            Edition = Sanitize(configuration.Get<string>(FieldEdition)),
        };

        return new(configuration.EntityId, generatorName, specs);
    }

    private static DataProcessorResult<IEntity?> ResultWithErrors(DataProcessorResult<IEntity?> original, List<Exception> errors)
        // Avoid creating a new record when no new exceptions were added.
        => errors.Count == original.Exceptions.Count
            ? original
            : original with { Exceptions = errors };

    private static bool IsContentTypeSchemaAction(string action)
        => action.EqualsInsensitive(PostSaveContentTypeCreate)
           || action.EqualsInsensitive(PostSaveContentTypeRename)
           || action.EqualsInsensitive(PostSaveContentTypeScopeChange)
           || action.EqualsInsensitive(PostSaveContentTypeFieldChange)
           || action.EqualsInsensitive(PostSaveContentTypeUpdate);

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

        // Normalize user-entered lists (comma/semicolon/newline separated) into a clean distinct set.
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

    /// <summary>
    /// Fully resolved generation job derived from one matching copilot configuration.
    /// </summary>
    private record RunConfiguration(
        int ConfigurationId,
        string GeneratorName,
        FileGeneratorSpecs Specs);
}
