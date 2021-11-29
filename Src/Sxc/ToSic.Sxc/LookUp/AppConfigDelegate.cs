using System;
using System.Collections.Specialized;
using System.Globalization;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Web;
using CmsBlock = ToSic.Sxc.DataSources.CmsBlock;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.LookUp
{
    public class AppConfigDelegate: HasLog<AppConfigDelegate>
    {
        #region Constructor / DI

        private readonly Lazy<ILookUpEngineResolver> _getEngineLazy;
        private readonly Lazy<IHttp> _httpLazy;

        public AppConfigDelegate(Lazy<ILookUpEngineResolver> getEngineLazy, Lazy<IHttp> httpLazy) : base("Sxc.CnfPrv")
        {
            _getEngineLazy = getEngineLazy;
            _httpLazy = httpLazy;
        }


        #endregion

        /// <summary>
        /// Generate a delegate which will be used to build the configuration based on a new sxc-instance
        /// </summary>
        internal Func<App, IAppDataConfiguration> BuildForNewBlock(IContextOfBlock context, IBlock block)
        {
            var showDrafts = context.UserMayEdit;

            var wrapLog = Log.Call($"showDrafts: {showDrafts}");


            wrapLog("ok");
            return appToUse =>
            {
                // check if we'll use the config already on the sxc-instance, or generate a new one
                var lookUpEngine = GetConfigProviderForModule(context, appToUse as IApp, block);

                // return results
                return new AppDataConfiguration(showDrafts, lookUpEngine);
            };
        }

        /// <summary>
        /// Generate a delegate which will be used to build the configuration based on a new sxc-instance
        /// </summary>
        internal Func<App, IAppDataConfiguration> Build(IBlock block)
        {
            var log = new Log("Sxc.CnfPrv", block.Log);
            var wrapLog = log.Call();
            var showDrafts = block.Context.UserMayEdit;
            var existingLookups = block.Data.Configuration.LookUpEngine;

            wrapLog("ok");
            return appToUse => new AppDataConfiguration(showDrafts, existingLookups);
        }

        /// <summary>
        /// Generate a delegate which will be used to build a basic configuration with very little context
        /// </summary>
        internal Func<App, IAppDataConfiguration> Build(bool showDrafts)
            => appToUse => new AppDataConfiguration(showDrafts,
                GetConfigProviderForModule(null, appToUse as IApp, null));



        // note: not sure yet where the best place for this method is, so it's here for now
        // will probably move again some day
        internal LookUpEngine GetConfigProviderForModule(IContextOfSite context, IApp appForLookup, IBlock blockForLookup)
        {
            var modId = (context as ContextOfBlock)?.Module.Id ?? 0;

            var wrapLog = Log.Call<LookUpEngine>($"{modId}, ..., ...");


            // Find the standard DNN property sources if PortalSettings object is available
            var envLookups = _getEngineLazy.Value.Init(Log).GetLookUpEngine(modId);
            Log.Add($"Environment provided {envLookups.Sources.Count} sources");

            var provider = new LookUpEngine(envLookups, Log);

            // Add QueryString etc. when running inside an http-context. Otherwise leave them away!
            var http = _httpLazy.Value;
            if (http.Current != null)
            {
                Log.Add("Found Http-Context, will ty to add params for querystring, server etc.");

                // new
                var paramList = new NameValueCollection();
                var ctxWithPage = context as IContextOfBlock;
                if (ctxWithPage?.Page.ParametersInternalOld != null)
                    foreach (var pair in ctxWithPage.Page.ParametersInternalOld)
                        paramList.Add(pair.Key, pair.Value);
                else
                    paramList = http.QueryStringParams;
                provider.Add(new LookUpInNameValueCollection("query", paramList));


                // old
#if NET451
                provider.Add(new LookUpInNameValueCollection("querystring", paramList));
                provider.Add(new LookUpInNameValueCollection("form", http.Request.Form));
                //provider.Add(new LookUpInNameValueCollection("server", http.Request.ServerVariables)); // deprecated
#else
                // "Not Yet Implemented in .net standard #TodoNetStandard" - might not actually support this
#endif
            }
            else
                Log.Add("No Http-Context found, won't add http params to look-up");


            provider.Add(new LookUpInAppProperty("app", appForLookup));

            // add module if it was not already added previously
            if (!provider.HasSource(CmsBlock.InstanceLookupName))
            {
                var modulePropertyAccess = new LookUpInDictionary(CmsBlock.InstanceLookupName);
                modulePropertyAccess.Properties.Add(CmsBlock.InstanceIdKey, modId.ToString(CultureInfo.InvariantCulture));
                provider.Add(modulePropertyAccess);
            }

            // provide the current SxcInstance to the children where necessary
            if (!provider.HasSource(LookUpConstants.InstanceContext) && blockForLookup != null)
            {
                var blockBuilderLookUp = new LookUpCmsBlock(LookUpConstants.InstanceContext, blockForLookup);
                provider.Add(blockBuilderLookUp);
            }
            return wrapLog("ok", provider);
        }
    }
}