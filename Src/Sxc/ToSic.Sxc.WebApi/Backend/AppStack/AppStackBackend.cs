using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Eav.Data.Sys.PropertyDump;
using ToSic.Eav.DataSource.Sys.Query;
using ToSic.Eav.DataSources.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using static ToSic.Eav.Apps.Sys.AppStack.AppStackConstants;

namespace ToSic.Sxc.Backend.AppStack;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppStackBackend(
    AppDataStackService dataStackService,
    IZoneCultureResolver zoneCulture,
    IAppReaderFactory appReaders,
    Generator<QueryDefinitionBuilder> qDefBuilder,
    IPropertyDumpService dumperService)
    : ServiceBase("Sxc.ApiApQ", connect: [dataStackService, zoneCulture, appReaders, dumperService])
{
    public List<AppStackDataRaw> GetAll(int appId, string part, string? key, Guid? viewGuid)
    {
        // Correct languages
        //if (languages == null || !languages.Any())
        var languages = zoneCulture.SafeLanguagePriorityCodes();
        // Get app 
        var appReader = appReaders.Get(appId);
        // Ensure we have the correct stack name
        var partName = SystemStackHelpers.GetStackNameOrNull(part);
        if (partName == null)
            throw new($"Parameter '{nameof(part)}' must be {RootNameSettings} or {RootNameResources}");
        var viewMixin = GetViewSettingsForMixin(viewGuid, languages, appReader, partName);
        var results = GetStackDump(appReader, partName, languages, viewMixin);
        
        results = SystemStackHelpers.ApplyKeysFilter(results, key);
        if (!results.Any())
            return [];

        var final = SystemStackHelpers.ReducePropertiesToRelevantOnes(results);

                

        return final.Select(r => new AppStackDataRaw(r))
            .ToList();
    }



    public List<PropertyDumpItem> GetStackDump(IAppReader appReader, string partName, string?[] languages, IEntity? viewSettingsMixin)
    {
        // Build Sources List
        var settings = dataStackService.Init(appReader).GetStack(partName, viewSettingsMixin);

        var results = dumperService.Dump(settings, new(null!, languages, true, Log), null!);
        return results;
    }


    private IEntity? GetViewSettingsForMixin(Guid? viewGuid, string?[] languages, IAppReadEntities appState, string realName)
    {
        if (viewGuid == null)
            return null;

        var viewEnt = appState.List.GetOne(viewGuid.Value)
                      ?? throw new($"Tried to get view but not found. Guid was {viewGuid}");
        
        var view = new View(viewEnt, languages, qDefBuilder);
        var viewStackPart = realName == RootNameSettings
            ? view.Settings
            : view.Resources;

        return viewStackPart;
    }
}