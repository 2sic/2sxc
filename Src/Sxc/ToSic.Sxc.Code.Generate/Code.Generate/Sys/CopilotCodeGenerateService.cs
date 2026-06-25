using ToSic.Eav.Apps;
using ToSic.Sys.DI;

namespace ToSic.Sxc.Code.Generate.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CopilotCodeGenerateService(
    FileSaver fileSaver,
    LazySvc<IEnumerable<IFileGenerator>> generators,
    IAppReaderFactory appReaders)
    : ServiceBase(SxcLogName + ".AutoGen.Run", connect: [fileSaver, generators, appReaders])
{
    internal const string DataCopilotConfigurationContentType = "DataCopilotConfiguration";
    internal const string FieldAutoGenerate = "AutoGenerate";
    internal const string FieldCodeGenerator = "CodeGenerator";
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

                specs = BuildFileGeneratorSpecs(configuration, specs);
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

    internal List<Exception> AutoGenerate(Job job)
    {
        var l = Log.Fn<List<Exception>>($"job:{job}");
        var errors = new List<Exception>();

        try
        {
            var generator = FindGenerator(job.GeneratorName);
            if (generator == null)
            {
                l.A($"Copilot auto-generate: generator '{job.GeneratorName}' not found.");
                errors.Add(new InvalidOperationException(
                    $"Generator '{job.GeneratorName}' not found for configuration '{job.ConfigurationId}'."));
                return l.Return(errors, "generator not found");
            }

            GenerateAndSave(generator, job.Specs);
        }
        catch (Exception ex)
        {
            errors.Add(ex);
            l.Ex(ex);
        }

        return l.Return(errors, $"errors:{errors.Count}");
    }

    private void GenerateAndSave(IFileGenerator generator, IFileGeneratorSpecs specs)
    {
        (generator as IHasLog)?.LinkLog(Log);
        fileSaver.GenerateAndSaveFiles(generator, specs);
    }

    private IFileGenerator? FindGenerator(string generatorName)
        => generators.Value.FirstOrDefault(g => g.Name.EqualsInsensitive(generatorName));

    internal static FileGeneratorSpecs BuildFileGeneratorSpecs(IEntity configuration, FileGeneratorSpecs baseSpecs)
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

    internal static ICollection<string>? GetSelectedContentTypes(IEntity configuration)
    {
        var selected = configuration.Get<string>(FieldContentTypes)
            .CsvToArrayWithoutEmpty()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return selected.Any()
            ? selected
            : null;
    }

    internal static string? Sanitize(string? value)
        => value?.Trim().NullIfNoValue();

    public record Result(bool Ok, string Message);

    internal record Job(
        int ConfigurationId,
        string GeneratorName,
        FileGeneratorSpecs Specs);
}
