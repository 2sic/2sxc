using System.Globalization;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Context;
using ToSic.Eav.LookUp;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Web.Internal.DotNet;
using CmsBlock = ToSic.Sxc.DataSources.CmsBlock;

namespace ToSic.Sxc.LookUp.Internal;

public class SxcAppDataConfigProvider(LazySvc<ILookUpEngineResolver> getEngineLazy, LazySvc<IHttp> httpLazy)
    : ServiceBase("Sxc.CnfPrv", connect: [getEngineLazy, httpLazy]), IAppDataConfigProvider
{
    public IAppDataConfiguration GetDataConfiguration(EavApp app, AppDataConfigSpecs specs)
    {
        var block = (specs as SxcAppDataConfigSpecs)?.BlockForLookupOrNull;
        var lookup = GetLookupEngineForContext(block?.Context, app as IApp, block);
        return new AppDataConfiguration(lookup, specs.ShowDrafts);
    }


    // note: not sure yet where the best place for this method is, so it's here for now
    // will probably move again some day
    internal LookUpEngine GetLookupEngineForContext(IContextOfSite context, IApp appForLookup, IBlock blockForLookupOrNull)
    {
        var l = Log.Fn<LookUpEngine>($"module: {(context as ContextOfBlock)?.Module.Id}, app: {appForLookup?.AppId} ..., ...");
        var modId = (context as ContextOfBlock)?.Module.Id ?? 0;

        // Find the standard DNN property sources if PortalSettings object is available
        var envLookups = getEngineLazy.Value.GetLookUpEngine(modId);
        var existSources = envLookups.Sources.ToList();
        l.A($"Environment provided {existSources.Count} sources");

        // Create a new lookup engine and add the standard sources as inner-sources
        //var provider = new LookUpEngine(envLookups, Log);

        var newSources = new List<ILookUp> { new LookUpInAppProperty("app", appForLookup) };

        // add module if it was not already added previously
        if (!existSources.HasSource(CmsBlock.InstanceLookupName))
        {
            var modulePropertyAccess = new LookUpInDictionary(CmsBlock.InstanceLookupName);
            modulePropertyAccess.Properties.Add(CmsBlock.ModuleIdKey, modId.ToString(CultureInfo.InvariantCulture));
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