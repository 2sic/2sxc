using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Services;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.DataSource.Query;
using ToSic.Eav.DataSources.Sys;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using static ToSic.Eav.Apps.AppStackConstants;

namespace ToSic.Sxc.WebApi.AppStack;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppStackBackend: ServiceBase
{

    #region Constructor / DI

    public AppStackBackend(AppDataStackService dataStackService, IZoneCultureResolver zoneCulture, IAppStates appStates, LazySvc<QueryDefinitionBuilder> qDefBuilder) : base("Sxc.ApiApQ")
    {
        ConnectServices(
            _dataStackService = dataStackService,
            _zoneCulture = zoneCulture,
            _appStates = appStates,
            _qDefBuilder = qDefBuilder
        );
    }

    private readonly IAppStates _appStates;
    private readonly IZoneCultureResolver _zoneCulture;
    private readonly AppDataStackService _dataStackService;
    private readonly LazySvc<QueryDefinitionBuilder> _qDefBuilder;

    #endregion

    public List<AppStackDataRaw> GetAll(int appId, string part, string key, Guid? viewGuid, string[] languages)
    {
        // Correct languages
        if (languages == null || !languages.Any())
            languages = _zoneCulture.SafeLanguagePriorityCodes();
        // Get app 
        var appState = _appStates.GetReader(appId);
        // Ensure we have the correct stack name
        var partName = SystemStackHelpers.GetStackNameOrNull(part);
        if (partName == null)
            throw new Exception($"Parameter '{nameof(part)}' must be {RootNameSettings} or {RootNameResources}");
        var viewMixin = GetViewSettingsForMixin(viewGuid, languages, appState, partName);
        var results = GetStackDump(appState, partName, languages, viewMixin);

        results = SystemStackHelpers.ApplyKeysFilter(results, key);
        if (!results.Any())
            return new List<AppStackDataRaw>();

        var final = SystemStackHelpers.ReducePropertiesToRelevantOnes(results);

                

        return final.Select(r => new AppStackDataRaw(r))
            .ToList();
    }



    public List<PropertyDumpItem> GetStackDump(IAppState appState, string partName, string[] languages, IEntity viewSettingsMixin)
    {
        // Build Sources List
        var settings = _dataStackService.Init(appState).GetStack(partName, viewSettingsMixin);

        // Dump results
        var results = settings._Dump(new PropReqSpecs(null, languages, Log), null);
        return results;
    }


    private IEntity GetViewSettingsForMixin(Guid? viewGuid, string[] languages, IAppEntityService appState, string realName)
    {
        IEntity viewStackPart = null;
        if (viewGuid != null)
        {
            var viewEnt = appState.List.One(viewGuid.Value);
            if (viewEnt == null) throw new Exception($"Tried to get view but not found. Guid was {viewGuid}");
            var view = new View(viewEnt, languages, Log, _qDefBuilder);

            viewStackPart = realName == RootNameSettings ? view.Settings : view.Resources;
        }

        return viewStackPart;
    }
}