using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.ContentBlocks;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.WebApi
{
    public static class Helpers
    {
        /// <summary>
        /// Workaround for deserializing KeyValuePair - it requires lowercase properties (case sensitive), which seems to be a bug in some Newtonsoft.Json versions: http://stackoverflow.com/questions/11266695/json-net-case-insensitive-property-deserialization
        /// </summary>
        public class UpperCaseStringKeyValuePair
        {
            public string Key { get; set; }
            public string Value{ get; set; }
        }

        internal static SxcInstance GetSxcOfApiRequest(HttpRequestMessage request, bool allowNoContextFound = false)
        {
            string cbidHeader = "ContentBlockId";
            var moduleInfo = request.FindModuleInfo();

            // get url parameters and provide override values to ensure all configuration is 
            // preserved in AJAX calls
            List<KeyValuePair<string, string>> urlParams = null;
            var requestParams = request.GetQueryNameValuePairs();
            var origParams = requestParams.Where(p => p.Key == "originalparameters").ToList();
            if (origParams.Any())
            {
                var paramSet = origParams.First().Value;

                // Workaround for deserializing KeyValuePair -it requires lowercase properties(case sensitive), which seems to be a bug in some Newtonsoft.Json versions: http://stackoverflow.com/questions/11266695/json-net-case-insensitive-property-deserialization
                var items = Json.Deserialize<List<UpperCaseStringKeyValuePair>>(paramSet);
                urlParams = items.Select(a => new KeyValuePair<string, string>(a.Key, a.Value)).ToList();
            }

            if (allowNoContextFound & moduleInfo == null)
                return null;

            IContentBlock contentBlock = new ModuleContentBlock(moduleInfo, urlParams);

            // check if we need an inner block
            if (request.Headers.Contains(cbidHeader)) { 
                var cbidh = request.Headers.GetValues(cbidHeader).FirstOrDefault();
                int cbid;
                Int32.TryParse(cbidh, out cbid);
                if (cbid < 0)   // negative id, so it's an inner block
                    contentBlock = new EntityContentBlock(contentBlock, cbid);
            }

            return contentBlock.SxcInstance;
        }


        internal static void RemoveLanguageChangingCookie()
        {
            // this is to fix a dnn-bug, which adds a cookie to set the language
            // but always defaults to the main language, which is simply wrong. 
            var cookies = System.Web.HttpContext.Current?.Response.Cookies;
            cookies?.Remove("language"); // try to remove, otherwise no exception will be thrown
        }
    }
}
