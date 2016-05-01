using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.ContentBlock;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.WebApi
{
    public static class HttpRequestMessageExtensions
    {
        internal static SxcInstance GetSxcOfModuleContext(this HttpRequestMessage request)
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
                var items = Json.Deserialize<List<KeyValuePair<string, string>>>(paramSet);
                urlParams = items.ToList();
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
