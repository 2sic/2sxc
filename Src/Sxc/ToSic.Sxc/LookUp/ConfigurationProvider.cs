using System;
using System.Collections.Specialized;
using System.Globalization;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Web;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.LookUp
{
    public class AppConfigDelegate: HasLog<AppConfigDelegate>
    {
        #region Constructor / DI

        private readonly Lazy<IGetEngine> _getEngineLazy;
        private readonly Lazy<IHttp> _httpLazy;

        public AppConfigDelegate(Lazy<IGetEngine> getEngineLazy, Lazy<IHttp> httpLazy) : base("Sxc.CnfPrv")
        {
            _getEngineLazy = getEngineLazy;
            _httpLazy = httpLazy;
        }
        

        #endregion


        /// <summary>
        /// Generate a delegate which will be used to build the configuration based on a new sxc-instance
        /// </summary>
        internal Func<App, IAppDataConfiguration> Build(IBlock block, bool useExistingConfig)
        {
            var log = new Log("Sxc.CnfPrv", block.Log);
            var wrapLog = log.Call($"{nameof(useExistingConfig)}:{useExistingConfig}");
            var containerId = block.Context.Container.Id;
            var showDrafts = block.EditAllowed;
            var activatePagePublishing = block.Context.ServiceProvider.Build<IPagePublishingResolver>()/*.Init(log)*/.IsEnabled(containerId);
            var existingLookups = block.Data.Configuration.LookUps;

            wrapLog("ok");
            return appToUse =>
            {
                // check if we'll use the config already on the sxc-instance, or generate a new one
                var lookUpEngine = useExistingConfig
                    ? existingLookups
                    : GetConfigProviderForModule(containerId, appToUse as IApp, block);

                // return results
                return new AppDataConfiguration(showDrafts, activatePagePublishing, lookUpEngine);
            };
        }

        /// <summary>
        /// Generate a delegate which will be used to build the configuration based existing stuff
        /// </summary>
        internal Func<App, IAppDataConfiguration> Build(bool showDrafts, bool publishingEnabled, ILookUpEngine config) 
            => appToUse => new AppDataConfiguration(showDrafts, publishingEnabled, config);

        /// <summary>
        /// Generate a delegate which will be used to build a basic configuration with very little context
        /// </summary>
        internal Func<App, IAppDataConfiguration> Build(bool showDrafts, bool publishingEnabled)
            => appToUse => new AppDataConfiguration(showDrafts, publishingEnabled,
                GetConfigProviderForModule(0, appToUse as IApp, null));



        // note: not sure yet where the best place for this method is, so it's here for now
        // will probably move again some day
        internal LookUpEngine GetConfigProviderForModule(int moduleId, IApp app, IBlock block)
        {
            var log = new Log("Stc.GetCnf", block?.Log);

            // Find the standard DNN property sources if PortalSettings object is available
            var envLookups = _getEngineLazy.Value.GetEngine(moduleId, block?.Log);
            log.Add($"Environment provided {envLookups.Sources.Count} sources");

            var provider = new LookUpEngine(envLookups, block?.Log);

            // Add QueryString etc. when running inside an http-context. Otherwise leave them away!
            var http = _httpLazy.Value;
            if (http.Current != null)
            {
                log.Add("Found Http-Context, will ty to add params for querystring, server etc.");

                // new
                var paramList = new NameValueCollection();
                if (block?.Context.Page.Parameters != null)
                    foreach (var pair in block.Context.Page.Parameters)
                        paramList.Add(pair.Key, pair.Value);
                else
                    paramList = http.QueryStringParams;
                provider.Add(new LookUpInNameValueCollection("querystring", paramList));

                // old
#if NET451
                provider.Add(new LookUpInNameValueCollection("server", http.Request.ServerVariables));
                provider.Add(new LookUpInNameValueCollection("form", http.Request.Form));
#else
                // "Not Yet Implemented in .net standard #TodoNetStandard" - might not actually support this
#endif
            }
            else
                log.Add("No Http-Context found, won't add http params to look-up");


            provider.Add(new LookUpInAppProperty("app", app));

            // add module if it was not already added previously
            if (!provider.HasSource(CmsBlock.InstanceLookupName))
            {
                var modulePropertyAccess = new LookUpInDictionary(CmsBlock.InstanceLookupName);
                modulePropertyAccess.Properties.Add(CmsBlock.InstanceIdKey, moduleId.ToString(CultureInfo.InvariantCulture));
                provider.Add(modulePropertyAccess);
            }

            // provide the current SxcInstance to the children where necessary
            if (!provider.HasSource(LookUpConstants.InstanceContext) && block != null)
            {
                var blockBuilderLookUp = new LookUpCmsBlock(LookUpConstants.InstanceContext, block);
                provider.Add(blockBuilderLookUp);
            }
            return provider;
        }
    }
}