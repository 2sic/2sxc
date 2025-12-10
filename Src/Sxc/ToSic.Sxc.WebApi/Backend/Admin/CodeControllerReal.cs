using System.Reflection;
using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Eav.Data.Sys.Entities;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sxc.Code.Sys.Documentation;
using ToSic.Sys.Utils.Assemblies;

namespace ToSic.Sxc.Backend.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class CodeControllerReal(FileSaver fileSaver, LazySvc<IEnumerable<IFileGenerator>> generators, LazySvc<IAppJsonConfigurationService> appJsonService, LazySvc<IAppReaderFactory> appReaders) 
    : ServiceBase("Api.CodeRl", connect: [appJsonService, appReaders])
{
    public const string LogSuffix = "Code";
    private const string DataCopilotConfigurationContentType = "DataCopilotConfiguration";

    public class HelpItem
    {
        // the name of the class
        public required string Term { get; set; }
        // message from the attribute
        public required string[] Help { get; set; }
    }

    public IEnumerable<HelpItem> InlineHelp(string language)
    {
        var l = Log.Fn<IEnumerable<HelpItem>>($"InlineHelp:l:{language}", timer: true);

        if (_inlineHelp != null)
            return l.ReturnAsOk(_inlineHelp);

        // TODO: stv# how to use languages?

        try
        {
            _inlineHelp = AssemblyHandling.GetTypes(Log)
                .Where(t => t != null!)
                .Where(t => t.IsDefined(typeof(DocsAttribute)))
                .Select(t => new HelpItem
                {
                    Term = t.Name,
                    Help = t.GetCustomAttribute<DocsAttribute>()?.GetMessages(t.FullName) ?? []
                })
                .ToArray();
            return l.ReturnAsOk(_inlineHelp);
        }
        catch (Exception e)
        {
            l.A("Exception in inline help.");
            l.Ex(e);
            return l.ReturnAsError([]);
        }
    }
    private static IEnumerable<HelpItem>? _inlineHelp;

    public RichResult GenerateDataModels(int appId, string? edition, string generator, int configurationId = 0)
    {
        var l = Log.Fn<RichResult>($"{nameof(appId)}:{appId};{nameof(edition)}:{edition}", timer: true);

        try
        {
            // find the generator
            var gen = generators.Value.FirstOrDefault(g => g.Name == generator);
            if (gen == null)
                return l.Return(new RichResult
                    {
                        Ok = false,
                        Message = $"Generator '{generator}' not found.",
                    }
                    .WithTime(l)
                );

            // Make sure the generator has the logger - if supported
            (gen as IHasLog)?.LinkLog(Log);

            // Determine the specs to generate with
            var specs = new FileGeneratorSpecs
            {
                AppId = appId,
                Edition = edition ?? ""
            };

            if (configurationId > 0)
            {
                var configuration = appReaders.Value.Get(appId).List.One(configurationId);
                if (configuration == null)
                    return l.Return(new RichResult
                        {
                            Ok = false,
                            Message = $"Configuration '{configurationId}' not found in app '{appId}'.",
                        }
                        .WithTime(l)
                    );

                // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                if (!DataCopilotConfigurationContentType.Equals(configuration.Type?.Name, StringComparison.OrdinalIgnoreCase))
                    return l.Return(new RichResult
                        {
                            Ok = false,
                            Message = $"Configuration '{configurationId}' is not a '{DataCopilotConfigurationContentType}' entity.",
                        }
                        .WithTime(l)
                    );

                specs = specs with
                {
                    Namespace = Sanitize(configuration.Get<string>("Namespace")),
                    TargetPath = Sanitize(configuration.Get<string>("TargetFolder")),
                    ContentTypes = Normalize(configuration.Get<string>("ContentTypes"))
                };
            }

            // generate and save files
            fileSaver.GenerateAndSaveFiles(gen, specs);

            return l.Return(new RichResult
                {
                    Ok = true,
                    Message = $"Data models generated in {specs.Edition}/{specs.TargetPath}.",
                }
                .WithTime(l)
            );
        }
        catch (Exception e)
        {
            return l.Return(new RichResult
                {
                    Ok = false,
                    Message = $"Error generating data models in {edition}/AppCode/Data. {e.GetType().FullName} - {e.Message}",
                }
                .WithTime(l)
            );
        }
    }

    private static string? Sanitize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value?.Trim();

    private static ICollection<string>? Normalize(string? raw)
    {
        var cleaned = Sanitize(raw);
        return cleaned == null ? null : Normalize([cleaned]);
    }

    private static ICollection<string>? Normalize(IEnumerable<string>? raw)
    {
        if (raw == null)
            return null;

        var cleaned = raw
            .SelectMany(item => item?
                .Split([',', ';', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
                ?? Array.Empty<string>())
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return cleaned.Any() ? cleaned : null;
    }

    public EditionsDto GetEditions(int appId)
    {
        var l = Log.Fn<EditionsDto>($"{nameof(appId)}:{appId}");

        // get generators
        var fileGenerators = generators.Value
            .Select(g => new GeneratorDto(g))
            .ToListOpt();

        var appJson = appJsonService.Value.GetAppJson(appId);
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        if (appJson?.Editions?.Count > 0)
        {
            l.A($"has editions in app.json: {appJson.Editions.Count}");
            return l.ReturnAsOk(appJson.ToEditionsDto(fileGenerators));
        }

        l.A("editions are not specified, so using default edition data");
        // default data
        var nothingSpecified = new EditionsDto
        {
            Ok = true,
            IsConfigured = false,
            Editions = [ new() { Name = "", Description = "Root edition", IsDefault = true } ],
            Generators = fileGenerators
        };

        return l.Return(nothingSpecified, "editions not specified in app.json");
    }
}
