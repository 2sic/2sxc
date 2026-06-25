using ToSic.Eav.Apps;
using ToSic.Eav.Data.Processing;
using ToSic.Sys.DI;

namespace ToSic.Sxc.Code.Generate.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CopilotContentTypeAutoGenerateService(
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

    public Result GenerateDataModels(int appId, string? edition, string generatorName, int configurationId = 0)
    {
        var l = Log.Fn<Result>($"{nameof(appId)}:{appId};{nameof(edition)}:{edition};{nameof(generatorName)}:{generatorName};{nameof(configurationId)}:{configurationId}", timer: true);

        try
        {
            var specs = new FileGeneratorSpecs
            {
                AppId = appId,
                Edition = edition ?? ""
            };

            if (configurationId > 0)
            {
                var configuration = appReaders.Get(appId).List.GetOne(configurationId);
                if (configuration == null)
                    return l.Return(new(false, $"Configuration '{configurationId}' not found in app '{appId}'."));

                // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                if (!DataCopilotConfigurationContentType.EqualsInsensitive(configuration.Type?.Name))
                    return l.Return(new(false, $"Configuration '{configurationId}' is not a '{DataCopilotConfigurationContentType}' entity."));

                var configuredGenerator = Sanitize(configuration.Get<string>(FieldCodeGenerator));
                if (configuredGenerator.HasValue())
                    generatorName = configuredGenerator;

                specs = BuildSpecs(configuration, specs);
            }

            var generator = FindGenerator(generatorName);
            if (generator == null)
                return l.Return(new(false, $"Generator '{generatorName}' not found."));

            GenerateAndSave(generator, specs);

            return l.Return(new(true, $"Data models generated in {specs.Edition}/{specs.TargetPath ?? "AppCode/Data"}."));
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return(new(false, $"Error generating data models in {edition}/AppCode/Data. {ex.GetType().FullName} - {ex.Message}"));
        }
    }

    public List<Exception> AutoGenerate(ContentTypeChange change)
    {
        var l = Log.Fn<List<Exception>>($"source:{change.Source}, app:{change.AppId}, type:{change.ContentTypeNameId}");
        var errors = new List<Exception>();

        var appReader = appReaders.Get(change.AppId);
        var changedType = appReader.GetContentType(change.ContentTypeNameId);

        var jobs = appReader.List
            .GetAll(DataCopilotConfigurationContentType)
            .Where(configuration => configuration.Get<bool>(FieldAutoGenerate))
            .Select(configuration => BuildJob(configuration, changedType))
            .Where(job => job != null)
            .Cast<Job>()
            .ToList();

        if (jobs.Count == 0)
        {
            l.A($"Copilot auto-generate: no matching configurations for content-type '{changedType.NameId}' ({change.Source}).");
            return l.Return(errors, "no matching auto-generate configurations");
        }

        foreach (var job in jobs)
        {
            try
            {
                var generator = FindGenerator(job.GeneratorName);
                if (generator == null)
                {
                    l.A($"Copilot auto-generate: generator '{job.GeneratorName}' not found.");
                    errors.Add(new InvalidOperationException(
                        $"Generator '{job.GeneratorName}' not found for configuration '{job.ConfigurationId}'."));
                    continue;
                }

                GenerateAndSave(generator, job.Specs);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                l.Ex(ex);
            }
        }

        return l.Return(errors, $"processed {jobs.Count} configuration(s)");
    }

    private void GenerateAndSave(IFileGenerator generator, IFileGeneratorSpecs specs)
    {
        (generator as IHasLog)?.LinkLog(Log);
        fileSaver.GenerateAndSaveFiles(generator, specs);
    }

    private IFileGenerator? FindGenerator(string generatorName)
        => generators.Value.FirstOrDefault(g => g.Name.EqualsInsensitive(generatorName));

    private static Job? BuildJob(IEntity configuration, IContentType changedType)
    {
        var generatorName = Sanitize(configuration.Get<string>(FieldCodeGenerator));
        if (generatorName.IsEmptyOrWs())
            return null;

        var selectedTypes = GetSelectedContentTypes(configuration);
        if (selectedTypes != null && !selectedTypes.Any(selection => selection.EqualsInsensitive(changedType.NameId)))
            return null;

        var specs = BuildSpecs(configuration, new()
        {
            AppId = changedType.AppId,
            ContentTypes = selectedTypes,
        });

        return new(configuration.EntityId, generatorName, specs);
    }

    private static FileGeneratorSpecs BuildSpecs(IEntity configuration, FileGeneratorSpecs baseSpecs)
        => baseSpecs with
        {
            Configuration = $"{configuration.EntityId} {configuration.GetBestTitle()}",
            Namespace = Sanitize(configuration.Get<string>(FieldNamespace)),
            TargetPath = Sanitize(configuration.Get<string>(FieldTargetFolder)),
            ContentTypes = baseSpecs.ContentTypes ?? GetSelectedContentTypes(configuration),
            Prefix = Sanitize(configuration.Get<string>(FieldPrefix)),
            Suffix = Sanitize(configuration.Get<string>(FieldSuffix)),
            Edition = Sanitize(configuration.Get<string>(FieldEdition)) ?? baseSpecs.Edition,
        };

    private static ICollection<string>? GetSelectedContentTypes(IEntity configuration)
    {
        var selected = configuration.Get<string>(FieldContentTypes)
            .CsvToArrayWithoutEmpty()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return selected.Any()
            ? selected
            : null;
    }

    private static string? Sanitize(string? value)
        => value?.Trim().NullIfNoValue();

    public record Result(bool Ok, string Message);

    private record Job(
        int ConfigurationId,
        string GeneratorName,
        FileGeneratorSpecs Specs);
}
