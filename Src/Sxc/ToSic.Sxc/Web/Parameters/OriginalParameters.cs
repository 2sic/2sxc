using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ToSic.Sxc.Web.Parameters
{

    public class OriginalParameters
    {
        public static string NameInUrlForOriginalParameters = "originalparameters";

        /// <summary>
        /// get url parameters and provide override values to ensure all configuration is 
        /// preserved in AJAX calls
        /// </summary>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetOverrideParams(List<KeyValuePair<string, string>> requestParams)
        {
            List<KeyValuePair<string, string>> urlParams = null;
            //var requestParams = request.GetQueryNameValuePairs();
            var origParams = requestParams.Where(p => p.Key == NameInUrlForOriginalParameters).ToList();
            if (!origParams.Any()) return requestParams;

            var paramSet = origParams.First().Value;

            // Workaround for deserializing KeyValuePair -it requires lowercase properties(case sensitive),
            // which seems to be a bug in some Newtonsoft.Json versions: http://stackoverflow.com/questions/11266695/json-net-case-insensitive-property-deserialization
            var items = JsonConvert.DeserializeObject<List<UpperCaseStringKeyValuePair>>(paramSet);
            urlParams = items.Select(a => new KeyValuePair<string, string>(a.Key, a.Value)).ToList();

            return urlParams;
        }
    }
}
