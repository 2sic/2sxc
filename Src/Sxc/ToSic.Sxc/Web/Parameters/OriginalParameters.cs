using System.Collections.Specialized;
using System.Text.Json;
using ToSic.Eav.Serialization;

namespace ToSic.Sxc.Web.Parameters;

internal class OriginalParameters
{
    public static string NameInUrlForOriginalParameters = "originalparameters";

    /// <summary>
    /// get url parameters and provide override values to ensure all configuration is 
    /// preserved in AJAX calls
    /// </summary>
    /// <param name="requestParams"></param>
    /// <returns></returns>
    public static NameValueCollection GetOverrideParams(NameValueCollection requestParams)
    {
        if (requestParams == null) return [];

        var paramSet = requestParams[NameInUrlForOriginalParameters];
        return string.IsNullOrEmpty(paramSet)
            ? requestParams  // just return requestParams (when origParams are not provided)
            : GetOverrideParams(paramSet);
    }

    public static NameValueCollection GetOverrideParams(string paramSet)
    {
        var urlParams = new NameValueCollection();

        // Workaround for deserializing KeyValuePair -it requires lowercase properties(case sensitive),
        // which seems to be a bug in some Newtonsoft.Json versions: http://stackoverflow.com/questions/11266695/json-net-case-insensitive-property-deserialization
        var items = JsonSerializer.Deserialize<List<UpperCaseStringKeyValuePair>>(paramSet, JsonOptions.SafeJsonForHtmlAttributes);
        items?.ForEach(a => urlParams.Add(a.Key, a.Value));

        return urlParams;
    }
}