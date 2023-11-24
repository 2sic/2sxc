using System;
using System.Collections.Specialized;
using System.Globalization;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Web;
using CmsBlock = ToSic.Sxc.DataSources.CmsBlock;
using IApp = ToSic.Sxc.Apps.IApp;
using ServiceBase = ToSic.Lib.Services.ServiceBase;

namespace ToSic.Sxc.LookUp;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppConfigDelegate : ServiceBase
{
    #region Constructor / DI

    private readonly LazySvc<ILookUpEngineResolver> _getEngineLazy;
    private readonly LazySvc<IHttp> _httpLazy;

    public AppConfigDelegate(LazySvc<ILookUpEngineResolver> getEngineLazy, LazySvc<IHttp> httpLazy) : base("Sxc.CnfPrv")
    {
        ConnectServices(
            _getEngineLazy = getEngineLazy,
            _httpLazy = httpLazy
        );
    }

    #endregion

    /// <summary>
    /// Generate a delegate which will be used to build the configuration based on a new sxc-instance
    /// </summary>
    internal Func<App, IAppDataConfiguration> BuildForNewBlock(IContextOfBlock context, IBlock block
    ) => Log.Func<Func<App, IAppDataConfiguration>>($"showDrafts: {context.UserMayEdit}", () =>
    {
        return appToUse =>
        {
            // check if we'll use the config already on the sxc-instance, or generate a new one
            var lookUpEngine = GetLookupEngineForContext(context, appToUse as IApp, block);

            // return results
            return new AppDataConfiguration(lookUpEngine);
        };
    });

    /// <summary>
    /// Generate a delegate which will be used to build the configuration based on a new sxc-instance
    /// </summary>
    internal Func<App, IAppDataConfiguration> Build(IBlock block) => block.Log.Func<Func<App, IAppDataConfiguration>>(() => appToUse => 
        new AppDataConfiguration(block.Data.Configuration.LookUpEngine));

    /// <summary>
    /// Generate a delegate which will be used to build a basic configuration with very little context
    /// </summary>
    internal Func<App, IAppDataConfiguration> Build(bool? showDrafts) => appToUse => 
        new AppDataConfiguration(GetLookupEngineForContext(null, appToUse as IApp, null), showDrafts);
    internal Func<App, IAppDataConfiguration> Build() => appToUse => 
        new AppDataConfiguration(GetLookupEngineForContext(null, appToUse as IApp, null));



    // note: not sure yet where the best place for this method is, so it's here for now
    // will probably move again some day
    internal LookUpEngine GetLookupEngineForContext(IContextOfSite context, IApp appForLookup, IBlock blockForLookup
    ) => Log.Func($"module: {(context as ContextOfBlock)?.Module.Id}, app: {appForLookup?.AppId} ..., ...", l =>
    {
        var modId = (context as ContextOfBlock)?.Module.Id ?? 0;

        // Find the standard DNN property sources if PortalSettings object is available
        var envLookups = _getEngineLazy.Value.GetLookUpEngine(modId);
        l.A($"Environment provided {envLookups.Sources.Count} sources");

        var provider = new LookUpEngine(envLookups, Log);

        // Add QueryString etc. when running inside an http-context. Otherwise leave them away!
        var http = _httpLazy.Value;
        if (http.Current != null)
        {
            l.A("Found Http-Context, will ty to add params for querystring, server etc.");

            // new (Oqt and Dnn)
            var paramList = new NameValueCollection();
            var ctxWithPage = context as IContextOfBlock;
            if (ctxWithPage?.Page.Parameters != null)
                foreach (var pair in ctxWithPage.Page.Parameters)
                    paramList.Add(pair.Key, pair.Value);
            else
                paramList = http.QueryStringParams;

            // add "query" if it was not already added previously (Oqt has it)
            if (!provider.HasSource(LookUpConstants.SourceQuery))
                provider.Add(new LookUpInNameValueCollection(LookUpConstants.SourceQuery, paramList));

#if NETFRAMEWORK
                // old (Dnn only)
                provider.Add(new LookUpInNameValueCollection(LookUpConstants.OldDnnSourceQueryString, paramList));
                provider.Add(new LookUpInNameValueCollection("form", http.Request.Form));
                //provider.Add(new LookUpInNameValueCollection("server", http.Request.ServerVariables)); // deprecated
#else
            // "Not Yet Implemented in .net standard #TodoNetStandard" - might not actually support this
#endif
        }
        else
            l.A("No Http-Context found, won't add http params to look-up");


        provider.Add(new LookUpInAppProperty("app", appForLookup));

        // add module if it was not already added previously
        if (!provider.HasSource(CmsBlock.InstanceLookupName))
        {
            var modulePropertyAccess = new LookUpInDictionary(CmsBlock.InstanceLookupName);
            modulePropertyAccess.Properties.Add(CmsBlock.ModuleIdKey, modId.ToString(CultureInfo.InvariantCulture));
            provider.Add(modulePropertyAccess);
        }

        // provide the current SxcInstance to the children where necessary
        if (!provider.HasSource(LookUpConstants.InstanceContext) && blockForLookup != null)
        {
            var blockBuilderLookUp = new LookUpCmsBlock(LookUpConstants.InstanceContext, blockForLookup);
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

        return (provider, $"{provider.Sources.Count}");
    });
}