using System.Globalization;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Eav.LookUp;
using ToSic.Eav.LookUp.Sources;
using ToSic.Eav.LookUp.Sources.Sys;
using ToSic.Eav.LookUp.Sys;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Web.Sys.Http;

namespace ToSic.Sxc.LookUp.Sys;

/// <summary>
/// Service to find the configuration for the AppData, especially the lookup...
/// </summary>
/// <param name="getEngineLazy"></param>
/// <param name="httpLazy"></param>
public class SxcAppDataConfigProvider(LazySvc<ILookUpEngineResolver> getEngineLazy, LazySvc<IHttp> httpLazy)
    : ServiceBase("Sxc.CnfPrv", connect: [getEngineLazy, httpLazy]), IAppDataConfigProvider
{
    public IAppDataConfiguration GetDataConfiguration(SxcAppBase app, AppDataConfigSpecs specs)
    {
        var block = (specs as SxcAppDataConfigSpecs)?.BlockForLookupOrNull;
        var lookup = GetLookupEngineForContext(block?.Context, app as IApp, block);
        return new AppDataConfiguration(lookup, specs.ShowDrafts);
    }


    // note: not sure yet where the best place for this method is, so it's here for now
    // will probably move again some day
    internal LookUpEngine GetLookupEngineForContext(IContextOfSite? context, IApp? appForLookup, IBlock? blockForLookupOrNull)
    {
        var l = Log.Fn<LookUpEngine>($"module: {(context as ContextOfBlock)?.Module.Id}, app: {appForLookup?.AppId} ..., ...");
        var modId = (context as ContextOfBlock)?.Module.Id ?? 0;

        // Find the standard DNN property sources if PortalSettings object is available
        var envLookups = getEngineLazy.Value.GetLookUpEngine(modId);
        var existSources = envLookups.Sources.ToList();
        l.A($"Environment provided {existSources.Count} sources");

        // Create a new lookup engine and add the standard sources as inner-sources
        var newSources = new List<ILookUp>();
        if (appForLookup != null)
            newSources.Add(new LookUpInAppProperty("app", appForLookup));

        // add module if it was not already added previously
        if (!existSources.HasSource(BlockInstanceConstants.InstanceLookupName))
        {
            var modulePropertyAccess = new LookUpInDictionary(BlockInstanceConstants.InstanceLookupName);
            modulePropertyAccess.Properties.Add(BlockInstanceConstants.ModuleIdKey, modId.ToString(CultureInfo.InvariantCulture));
            newSources.Add(modulePropertyAccess);
        }

        // provide the current SxcInstance to the children where necessary
        if (!existSources.HasSource(LookUpConstants.InstanceContext) && blockForLookupOrNull != null)
        {
            var blockBuilderLookUp = new LookUpCmsBlock(LookUpConstants.InstanceContext, blockForLookupOrNull);
            newSources.Add(blockBuilderLookUp);
        }

        // Provide the settings & resources stack lookup
        if (context is IContextOfApp contextOfApp)
        {
            l.A("Inside an App-Context, will add stack sources for settings/resources");
            var dims = contextOfApp.Site.SafeLanguagePriorityCodes();
            newSources.Add(new LookUpInStack(contextOfApp.AppSettings, dims));
            newSources.Add(new LookUpInStack(contextOfApp.AppResources, dims));
        }
        else
            l.A("Not in App context, will not add stack for settings/resources");

        var provider = new LookUpEngine(envLookups, Log, sources: newSources);

        return l.Return(provider, $"{provider.Sources.Count()}");
    }

}