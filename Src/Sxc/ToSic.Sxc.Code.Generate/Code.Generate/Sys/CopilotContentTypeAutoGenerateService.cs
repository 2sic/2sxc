using ToSic.Eav.Apps;
using ToSic.Sys.DI;

namespace ToSic.Sxc.Code.Generate.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CopilotContentTypeAutoGenerateService(
    FileSaver fileSaver,
    LazySvc<IEnumerable<IFileGenerator>> generators,
    IAppReaderFactory appReaders)
    : ServiceBase(SxcLogName + ".AutoGen.Run", connect: [fileSaver, generators, appReaders])
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

    public List<Exception> Generate(int appId, string changedTypeNameId, string origin)
    {
        var l = Log.Fn<List<Exception>>($"origin:{origin}, app:{appId}, type:{changedTypeNameId}");
        var errors = new List<Exception>();

        if (changedTypeNameId.IsEmptyOrWs())
        {
            Log.A("Copilot auto-generate skipped: content-type name-id is empty.");
            return l.Return(errors, "missing content-type name-id");
        }

        var appReader = appReaders.Get(appId);
        var changedType = appReader.TryGetContentType(changedTypeNameId);
        if (changedType == null)
        {
            errors.Add(new ArgumentException(
                $"Content-Type '{changedTypeNameId}' not found in app '{appId}'."));
            Log.A($"Copilot auto-generate skipped: content-type '{changedTypeNameId}' not found in app '{appId}'.");
            return l.Return(errors, "content-type not found");
        }

        var matchingConfigurations = appReader.List
            .GetAll(DataCopilotConfigurationContentType)
            .Where(configuration => configuration.Get<bool>(FieldAutoGenerate))
            .Select(configuration => BuildRunConfiguration(configuration, changedType))
            .Where(configuration => configuration != null)
            .Cast<RunConfiguration>()
            .ToList();

        if (!matchingConfigurations.Any())
        {
            Log.A($"Copilot auto-generate: no matching configurations for content-type '{changedType.NameId}' ({origin}).");
            return l.Return(errors, "no matching auto-generate configurations");
        }

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
                    continue;
                }

                (generator as IHasLog)?.LinkLog(Log);
                fileSaver.GenerateAndSaveFiles(generator, configuration.Specs);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                Log.Ex(ex);
            }
        }

        return l.Return(errors, $"processed {matchingConfigurations.Count} configuration(s)");
    }

    private static RunConfiguration? BuildRunConfiguration(IEntity configuration, IContentType changedType)
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

    private record RunConfiguration(
        int ConfigurationId,
        string GeneratorName,
        FileGeneratorSpecs Specs);
}
