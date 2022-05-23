using System;
using System.Collections.Specialized;
using System.Globalization;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Web;
using CmsBlock = ToSic.Sxc.DataSources.CmsBlock;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.LookUp
{
    public class AppConfigDelegate : HasLog/*<AppConfigDelegate>*/
    {
        #region Constructor / DI

        private readonly LazyInitLog<ILookUpEngineResolver> _getEngineLazy;
        private readonly Lazy<IHttp> _httpLazy;

        public AppConfigDelegate(LazyInitLog<ILookUpEngineResolver> getEngineLazy, Lazy<IHttp> httpLazy) : base("Sxc.CnfPrv")
        {
            _getEngineLazy = getEngineLazy.SetLog(Log);
            _httpLazy = httpLazy;
        }


        #endregion

        /// <summary>
        /// Generate a delegate which will be used to build the configuration based on a new sxc-instance
        /// </summary>
        internal Func<App, IAppDataConfiguration> BuildForNewBlock(IContextOfBlock context, IBlock block)
        {
            var showDrafts = context.UserMayEdit;

            var wrapLog = Log.Fn<Func<App, IAppDataConfiguration>>($"showDrafts: {showDrafts}");

            return wrapLog.Return(appToUse =>
            {
                // check if we'll use the config already on the sxc-instance, or generate a new one
                var lookUpEngine = GetLookupEngineForContext(context, appToUse as IApp, block);

                // return results
                return new AppDataConfiguration(showDrafts, lookUpEngine);
            },"ok");
        }

        /// <summary>
        /// Generate a delegate which will be used to build the configuration based on a new sxc-instance
        /// </summary>
        internal Func<App, IAppDataConfiguration> Build(IBlock block)
        {
            var log = new Log("Sxc.CnfPrv", block.Log);
            var wrapLog = log.Fn<Func<App, IAppDataConfiguration>>();
            var showDrafts = block.Context.UserMayEdit;
            var existingLookups = block.Data.Configuration.LookUpEngine;
            return wrapLog.Return(appToUse => new AppDataConfiguration(showDrafts, existingLookups), "ok");
        }

        /// <summary>
        /// Generate a delegate which will be used to build a basic configuration with very little context
        /// </summary>
        internal Func<App, IAppDataConfiguration> Build(bool showDrafts)
            => appToUse => new AppDataConfiguration(showDrafts,
                GetLookupEngineForContext(null, appToUse as IApp, null));



        // note: not sure yet where the best place for this method is, so it's here for now
        // will probably move again some day
        internal LookUpEngine GetLookupEngineForContext(IContextOfSite context, IApp appForLookup, IBlock blockForLookup)
        {
            var modId = (context as ContextOfBlock)?.Module.Id ?? 0;

            var wrapLog = Log.Fn<LookUpEngine>($"module: {modId}, app: {appForLookup?.AppId} ..., ...");


            // Find the standard DNN property sources if PortalSettings object is available
            var envLookups = _getEngineLazy.Ready.GetLookUpEngine(modId);
            Log.A($"Environment provided {envLookups.Sources.Count} sources");

            var provider = new LookUpEngine(envLookups, Log);

            // Add QueryString etc. when running inside an http-context. Otherwise leave them away!
            var http = _httpLazy.Value;
            if (http.Current != null)
            {
                Log.A("Found Http-Context, will ty to add params for querystring, server etc.");


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
                Log.A("No Http-Context found, won't add http params to look-up");


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
            return wrapLog.Return(provider, "ok");
        }
    }
}
