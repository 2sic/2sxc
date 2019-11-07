using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Engines.Token;
using ToSic.Sxc.Interfaces;

namespace ToSic.SexyContent.DataSources
{
    public class ConfigurationProvider
    {
        /// <summary>
        /// This is the key in the dictionary of providers, which contains the special one
        /// meant to transport sxc-instance info to other sources needing it
        /// </summary>
        public const string SxcInstanceKey = "SxcInstance";

        /// <summary>
        /// Generate a delegate which will be used to build the configuration based on a new sxc-instance
        /// </summary>
        internal static Func<Eav.Apps.App, IAppDataConfiguration> Build(SxcInstance sxcInstance, bool useExistingConfig)
        {
            return appToUse =>
            {
                // the module id
                var envInstanceId = sxcInstance.EnvInstance.Id;

                // check if we'll use the config already on the sxc-instance, or generate a new one
                var config = useExistingConfig
                    ? sxcInstance.Data.ConfigurationProvider
                    : GetConfigProviderForModule(envInstanceId, appToUse as App, sxcInstance);

                // return results
                return new AppDataConfiguration(sxcInstance.UserMayEdit,
                    sxcInstance.Environment.PagePublishing.IsEnabled(envInstanceId), config);
            };
        }

        /// <summary>
        /// Generate a delegate which will be used to build the configuration based existing stuff
        /// </summary>
        internal static Func<Eav.Apps.App, IAppDataConfiguration> Build(bool userMayEdit, bool publishEnabled, ITokenListFiller config) 
            => appToUse => new AppDataConfiguration(userMayEdit, publishEnabled, config);

        /// <summary>
        /// Generate a delegate which will be used to build a basic configuration with very little context
        /// </summary>
        internal static Func<Eav.Apps.App, IAppDataConfiguration> Build(bool userMayEdit, bool publishEnabled)
            => appToUse => new AppDataConfiguration(userMayEdit, publishEnabled,
                GetConfigProviderForModule(0, appToUse as App, null));



        // note: not sure yet where the best place for this method is, so it's here for now
        // will probably move again some day
        internal static TokenListFiller GetConfigProviderForModule(int moduleId, App app, SxcInstance sxc)
        {
            var provider = new TokenListFiller();

            // only add these in running inside an http-context. Otherwise leave them away!
            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Request;

                // new
                var paramList = new NameValueCollection();
                if(sxc?.Parameters != null)
                    foreach (var pair in sxc.Parameters)
                        paramList.Add(pair.Key, pair.Value);
                else
                    paramList = request.QueryString;
                provider.Sources.Add("querystring", new LookUpInNameValueCollection("querystring", paramList));

                // old
                provider.Sources.Add("server", new LookUpInNameValueCollection("server", request.ServerVariables));
                provider.Sources.Add("form", new LookUpInNameValueCollection("form", request.Form));
            }

            // Add the standard DNN property sources if PortalSettings object is available (changed 2018-03-05)
            var envProvs = Factory.Resolve<IEnvironmentLookUps>().GetLookUps(moduleId).Sources;
            foreach (var prov in envProvs)
                provider.Sources.Add(prov.Key, prov.Value);

            provider.Sources.Add("app", new LookUpInAppProperty("app", app));

            // add module if it was not already added previously
            if (!provider.Sources.ContainsKey("module"))
            {
                var modulePropertyAccess = new LookUpInDictionary("module");
                modulePropertyAccess.Properties.Add("ModuleID", moduleId.ToString(CultureInfo.InvariantCulture));
                provider.Sources.Add(modulePropertyAccess.Name, modulePropertyAccess);
            }

            // provide the current SxcInstance to the children where necessary
            if (!provider.Sources.ContainsKey(SxcInstanceKey))
            {
                var sxci = new SxcInstanceLookUp(SxcInstanceKey, null, sxc);
                provider.Sources.Add(sxci.Name, sxci);
            }
            return provider;
        }
    }
}