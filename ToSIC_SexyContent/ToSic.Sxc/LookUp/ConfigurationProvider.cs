using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Blocks;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.LookUp
{
    public class ConfigurationProvider
    {
        /// <summary>
        /// Generate a delegate which will be used to build the configuration based on a new sxc-instance
        /// </summary>
        internal static Func<App, IAppDataConfiguration> Build(IBlockBuilder blockBuilder, bool useExistingConfig)
        {
            return appToUse =>
            {
                // the module id
                var envInstanceId = blockBuilder.Container.Id;

                // check if we'll use the config already on the sxc-instance, or generate a new one
                var config = useExistingConfig
                    ? blockBuilder.Block.Data.Configuration.LookUps
                    : GetConfigProviderForModule(envInstanceId, appToUse as IApp, blockBuilder);

                // return results
                return new AppDataConfiguration(blockBuilder.UserMayEdit,
                    blockBuilder.Environment.PagePublishing.IsEnabled(envInstanceId), config);
            };
        }

        /// <summary>
        /// Generate a delegate which will be used to build the configuration based existing stuff
        /// </summary>
        internal static Func<App, IAppDataConfiguration> Build(bool userMayEdit, bool publishingEnabled, ILookUpEngine config) 
            => appToUse => new AppDataConfiguration(userMayEdit, publishingEnabled, config);

        /// <summary>
        /// Generate a delegate which will be used to build a basic configuration with very little context
        /// </summary>
        internal static Func<App, IAppDataConfiguration> Build(bool userMayEdit, bool publishingEnabled)
            => appToUse => new AppDataConfiguration(userMayEdit, publishingEnabled,
                GetConfigProviderForModule(0, appToUse as IApp, null));



        // note: not sure yet where the best place for this method is, so it's here for now
        // will probably move again some day
        internal static LookUpEngine GetConfigProviderForModule(int moduleId, IApp app, IBlockBuilder blockBuilder)
        {
            var log = new Log("Stc.GetCnf", blockBuilder?.Log);

            // Find the standard DNN property sources if PortalSettings object is available (changed 2018-03-05)
            var dnnLookUps = Factory.Resolve<IGetEngine>().GetEngine(moduleId, blockBuilder?.Log);
            log.Add($"Environment provided {dnnLookUps.Sources.Count} sources");

            var provider = new LookUpEngine(dnnLookUps, blockBuilder?.Log);

            // only add these in running inside an http-context. Otherwise leave them away!
            if (HttpContext.Current != null)
            {
                log.Add("Found HttpContext, will ty to add params for querystring, server etc.");
                var request = HttpContext.Current.Request;

                // new
                var paramList = new NameValueCollection();
                if (blockBuilder?.Parameters != null)
                    foreach (var pair in blockBuilder.Parameters)
                        paramList.Add(pair.Key, pair.Value);
                else
                    paramList = request.QueryString;
                provider.Add(new LookUpInNameValueCollection("querystring", paramList));

                // old
                provider.Add(new LookUpInNameValueCollection("server", request.ServerVariables));
                provider.Add(new LookUpInNameValueCollection("form", request.Form));
            }
            else
                log.Add("No HttpContext found, won't add http params to look-up");


            provider.Add(new LookUpInAppProperty("app", app));

            // add module if it was not already added previously
            if (!provider.HasSource("module"))
            {
                var modulePropertyAccess = new LookUpInDictionary("module");
                modulePropertyAccess.Properties.Add("ModuleID", moduleId.ToString(CultureInfo.InvariantCulture));
                provider.Add(modulePropertyAccess);
            }

            // provide the current SxcInstance to the children where necessary
            if (!provider.HasSource(LookUpConstants.InstanceContext) && blockBuilder != null)
            {
                var blockBuilderLookUp = new LookUpCmsBlock(LookUpConstants.InstanceContext, blockBuilder);
                provider.Add(blockBuilderLookUp);
            }
            return provider;
        }
    }
}