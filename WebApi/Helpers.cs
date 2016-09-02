using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.ContentBlock;
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

        internal static SxcInstance GetSxcOfApiRequest(HttpRequestMessage request)
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

                //urlParams = requestParams
                //    .Where(keyValuePair => keyValuePair.Key.IndexOf("orig", StringComparison.Ordinal) == 0)
                //    .Select(pair => new KeyValuePair<string, string>(pair.Key.Substring(4), pair.Value))
                //    .ToList();
            }
            // first, check the special overrides
            //var origparams = requestParams.Select(np => np.Key == "urlparameters").ToList();
            //if (origparams.Any())
            //{
            //    var paramSet = origparams.First();
            //}

            // then add remaining params


            IContentBlock contentBlock = new ModuleContentBlock(moduleInfo, urlParams);

            // check if we need an inner block
            if (request.Headers.Contains(cbidHeader)) { 
                var cbidh = request.Headers.GetValues(cbidHeader).FirstOrDefault();
                int cbid;
                int.TryParse(cbidh, out cbid);
                if (cbid < 0)   // negative id, so it's an inner block
                    contentBlock = new EntityContentBlock(contentBlock, cbid);
            }

            return contentBlock.SxcInstance;
        }

    }
}
