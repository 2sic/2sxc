using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Api;
using ToSic.Eav.Logging;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn;

namespace ToSic.Sxc.WebApi
{
    public static class Helpers
    {
        /// <summary>
        /// Workaround for deserializing KeyValuePair - it requires lowercase properties (case sensitive), 
        /// which seems to be a issue in some Newtonsoft.Json versions: http://stackoverflow.com/questions/11266695/json-net-case-insensitive-property-deserialization
        /// </summary>
        public class UpperCaseStringKeyValuePair
        {
            public string Key { get; set; }
            public string Value{ get; set; }
        }

        internal static ICmsBlock GetCmsBlock(HttpRequestMessage request, bool allowNoContextFound, ILog log)
        {
            var wrapLog = log.Call<ICmsBlock>(parameters: $"request:..., {nameof(allowNoContextFound)}: {allowNoContextFound}");

            const string headerId = "ContentBlockId";
            var moduleInfo = request.FindModuleInfo();

            if (allowNoContextFound & moduleInfo == null)
                return wrapLog("not found, allowed", null);
            
            if (moduleInfo == null)
                log.Add("context/module not found");

            var urlParams = PrepareUrlParamsForInternalUse(request);
            
            var tenant = moduleInfo == null
                ? new Tenant(null)
                : new Tenant(new PortalSettings(moduleInfo.OwnerPortalID));

            IBlock contentBlock = new BlockFromModule(new DnnContainer(moduleInfo), log, tenant, urlParams);

            // check if we need an inner block
            if (request.Headers.Contains(headerId)) { 
                var blockHeaderId = request.Headers.GetValues(headerId).FirstOrDefault();
                int.TryParse(blockHeaderId, out var blockId);
                if (blockId < 0)   // negative id, so it's an inner block
                {
                    log.Add($"Inner Content: {blockId}");
                    contentBlock = new BlockFromEntity(contentBlock, blockId, log);
                }
            }

            return wrapLog("ok", contentBlock.CmsInstance);
        }

        /// <summary>
        /// get url parameters and provide override values to ensure all configuration is 
        /// preserved in AJAX calls
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static List<KeyValuePair<string, string>> PrepareUrlParamsForInternalUse(HttpRequestMessage request)
        {
            List<KeyValuePair<string, string>> urlParams = null;
            var requestParams = request.GetQueryNameValuePairs();
            var origParams = requestParams.Where(p => p.Key == "originalparameters").ToList();
            if (origParams.Any())
            {
                var paramSet = origParams.First().Value;

                // Workaround for deserializing KeyValuePair -it requires lowercase properties(case sensitive),
                // which seems to be a bug in some Newtonsoft.Json versions: http://stackoverflow.com/questions/11266695/json-net-case-insensitive-property-deserialization
                var items = Json.Deserialize<List<UpperCaseStringKeyValuePair>>(paramSet);
                urlParams = items.Select(a => new KeyValuePair<string, string>(a.Key, a.Value)).ToList();
            }

            return urlParams;
        }


        internal static void RemoveLanguageChangingCookie()
        {
            // this is to fix a dnn-bug, which adds a cookie to set the language
            // but always defaults to the main language, which is simply wrong. 
            var cookies = global::System.Web.HttpContext.Current?.Response.Cookies;
            cookies?.Remove("language"); // try to remove, otherwise no exception will be thrown
        }
    }
}
