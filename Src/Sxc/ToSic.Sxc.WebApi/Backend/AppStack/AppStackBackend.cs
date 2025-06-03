﻿using ToSic.Eav.Apps.Services;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyDump.Sys;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.DataSources.Sys.Internal;
using ToSic.Sxc.Blocks.Internal;
using static ToSic.Eav.Apps.AppStackConstants;

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
    public List<AppStackDataRaw> GetAll(int appId, string part, string key, Guid? viewGuid)
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



    public List<PropertyDumpItem> GetStackDump(IAppReader appReader, string partName, string[] languages, IEntity viewSettingsMixin)
    {
        // Build Sources List
        var settings = dataStackService.Init(appReader).GetStack(partName, viewSettingsMixin);

        // Dump results
        // #DropUseOfDumpProperties
        //var results = settings._DumpNameWipDroppingMostCases(new(null, languages, true, Log), null);

        var results = dumperService.Dump(settings, new(null, languages, true, Log), null);
        return results;
    }


    private IEntity GetViewSettingsForMixin(Guid? viewGuid, string[] languages, IAppReadEntities appState, string realName)
    {
        if (viewGuid == null)
            return null;

        var viewEnt = appState.List.One(viewGuid.Value)
                      ?? throw new($"Tried to get view but not found. Guid was {viewGuid}");
        
        var view = new View(viewEnt, languages, Log, qDefBuilder);
        var viewStackPart = realName == RootNameSettings
            ? view.Settings
            : view.Resources;

        return viewStackPart;
    }
}