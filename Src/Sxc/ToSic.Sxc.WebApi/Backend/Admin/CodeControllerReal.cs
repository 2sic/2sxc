﻿using System.Reflection;
using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sxc.Code.Sys.Documentation;
using ToSic.Sys.Utils.Assemblies;

namespace ToSic.Sxc.Backend.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class CodeControllerReal(FileSaver fileSaver, LazySvc<IEnumerable<IFileGenerator>> generators, LazySvc<IAppJsonConfigurationService> appJsonService) 
    : ServiceBase("Api.CodeRl", connect: [appJsonService])
{
    public const string LogSuffix = "Code";

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

    public RichResult GenerateDataModels(int appId, string edition, string generator)
    {
        var l = Log.Fn<RichResult>($"{nameof(appId)}:{appId};{nameof(edition)}:{edition}", timer: true);

        try
        {
            var specs = new FileGeneratorSpecs { AppId = appId, Edition = edition };

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

            // generate and save files
            fileSaver.GenerateAndSaveFiles(gen, specs);

            return l.Return(new RichResult
                {
                    Ok = true,
                    Message = $"Data models generated in {edition}/AppCode/Data.",
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

    public EditionsDto GetEditions(int appId)
    {
        var l = Log.Fn<EditionsDto>($"{nameof(appId)}:{appId}");

        // get generators
        var fileGenerators = generators.Value
            .Select(g => new GeneratorDto(g))
            .ToListOpt();

        var appJson = appJsonService.Value.GetAppJson(appId);
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