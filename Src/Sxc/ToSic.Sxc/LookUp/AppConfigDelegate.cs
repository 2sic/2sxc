using System.Globalization;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Web.Internal.DotNet;
using CmsBlock = ToSic.Sxc.DataSources.CmsBlock;
using IApp = ToSic.Sxc.Apps.IApp;
using ServiceBase = ToSic.Lib.Services.ServiceBase;

namespace ToSic.Sxc.LookUp;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppConfigDelegate(LazySvc<ILookUpEngineResolver> getEngineLazy, LazySvc<IHttp> httpLazy)
    : ServiceBase("Sxc.CnfPrv", connect: [getEngineLazy, httpLazy])
{

    /// <summary>
    /// Generate a delegate which will be used to build the configuration based on a new sxc-instance
    /// </summary>
    internal Func<EavApp, IAppDataConfiguration> BuildForNewBlock(IContextOfBlock context, IBlock block)
    {
        var l = Log.Fn<Func<EavApp, IAppDataConfiguration>>($"showDrafts: {context.UserMayEdit}");
        return l.Return(appToUse =>
        {
            // check if we'll use the config already on the sxc-instance, or generate a new one
            var lookUpEngine = GetLookupEngineForContext(context, appToUse as IApp, block);

            // return results
            return new AppDataConfiguration(lookUpEngine);
        });
    }

    /// <summary>
    /// Generate a delegate which will be used to build a basic configuration with very little context
    /// </summary>
    internal Func<EavApp, IAppDataConfiguration> Build(bool? showDrafts) => appToUse => 
        new AppDataConfiguration(GetLookupEngineForContext(null, appToUse as IApp, null), showDrafts);

    internal Func<EavApp, IAppDataConfiguration> Build() => appToUse => 
        new AppDataConfiguration(GetLookupEngineForContext(null, appToUse as IApp, null));



    // note: not sure yet where the best place for this method is, so it's here for now
    // will probably move again some day
    internal LookUpEngine GetLookupEngineForContext(IContextOfSite context, IApp appForLookup, IBlock blockForLookupOrNull) 
    {
        var l = Log.Fn<LookUpEngine>($"module: {(context as ContextOfBlock)?.Module.Id}, app: {appForLookup?.AppId} ..., ...");
        var modId = (context as ContextOfBlock)?.Module.Id ?? 0;

        // Find the standard DNN property sources if PortalSettings object is available
        var envLookups = getEngineLazy.Value.GetLookUpEngine(modId);
        l.A($"Environment provided {envLookups.Sources.Count} sources");

        // Create a new lookup engine and add the standard sources as inner-sources
        var provider = new LookUpEngine(envLookups, Log);

        provider.Add(new LookUpInAppProperty("app", appForLookup));

        // add module if it was not already added previously
        if (!provider.HasSource(CmsBlock.InstanceLookupName))
        {
            var modulePropertyAccess = new LookUpInDictionary(CmsBlock.InstanceLookupName);
            modulePropertyAccess.Properties.Add(CmsBlock.ModuleIdKey, modId.ToString(CultureInfo.InvariantCulture));
            provider.Add(modulePropertyAccess);
        }

        // provide the current SxcInstance to the children where necessary
        if (!provider.HasSource(LookUpConstants.InstanceContext) && blockForLookupOrNull != null)
        {
            var blockBuilderLookUp = new LookUpCmsBlock(LookUpConstants.InstanceContext, blockForLookupOrNull);
            provider.Add(blockBuilderLookUp);
        }

        // Provide the settings & resources stack lookup
        if (context is IContextOfApp contextOfApp)
        {
            l.A("Inside an App-Context, will add stack sources for settings/resources");
            var dims = contextOfApp.Site.SafeLanguagePriorityCodes();
            provider.Add(new LookUpInStack(contextOfApp.AppSettings, dims));
            provider.Add(new LookUpInStack(contextOfApp.AppResources, dims));
        }
        else
            l.A("Not in App context, will not add stack for settings/resources");

        return l.Return(provider, $"{provider.Sources.Count}");
    }


}