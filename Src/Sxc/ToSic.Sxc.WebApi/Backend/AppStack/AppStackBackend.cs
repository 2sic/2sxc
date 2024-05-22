using ToSic.Eav.Apps.Services;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.DataSources.Sys.Internal;
using ToSic.Sxc.Blocks.Internal;
using static ToSic.Eav.Apps.AppStackConstants;

namespace ToSic.Sxc.Backend.AppStack;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppStackBackend(
    AppDataStackService dataStackService,
    IZoneCultureResolver zoneCulture,
    IAppStates appStates,
    Generator<QueryDefinitionBuilder> qDefBuilder)
    : ServiceBase("Sxc.ApiApQ", connect: [dataStackService, zoneCulture, appStates])
{
    public List<AppStackDataRaw> GetAll(int appId, string part, string key, Guid? viewGuid, string[] languages)
    {
        // Correct languages
        if (languages == null || !languages.Any())
            languages = zoneCulture.SafeLanguagePriorityCodes();
        // Get app 
        var appState = appStates.GetReader(appId);
        // Ensure we have the correct stack name
        var partName = SystemStackHelpers.GetStackNameOrNull(part);
        if (partName == null)
            throw new($"Parameter '{nameof(part)}' must be {RootNameSettings} or {RootNameResources}");
        var viewMixin = GetViewSettingsForMixin(viewGuid, languages, appState, partName);
        var results = GetStackDump(appState, partName, languages, viewMixin);

        results = SystemStackHelpers.ApplyKeysFilter(results, key);
        if (!results.Any())
            return [];

        var final = SystemStackHelpers.ReducePropertiesToRelevantOnes(results);

                

        return final.Select(r => new AppStackDataRaw(r))
            .ToList();
    }



    public List<PropertyDumpItem> GetStackDump(IAppState appState, string partName, string[] languages, IEntity viewSettingsMixin)
    {
        // Build Sources List
        var settings = dataStackService.Init(appState).GetStack(partName, viewSettingsMixin);

        // Dump results
        var results = settings._Dump(new(null, languages, Log), null);
        return results;
    }


    private IEntity GetViewSettingsForMixin(Guid? viewGuid, string[] languages, IAppEntityService appState, string realName)
    {
        IEntity viewStackPart = null;
        if (viewGuid != null)
        {
            var viewEnt = appState.List.One(viewGuid.Value);
            if (viewEnt == null) throw new($"Tried to get view but not found. Guid was {viewGuid}");
            var view = new View(viewEnt, languages, Log, qDefBuilder);

            viewStackPart = realName == RootNameSettings ? view.Settings : view.Resources;
        }

        return viewStackPart;
    }
}