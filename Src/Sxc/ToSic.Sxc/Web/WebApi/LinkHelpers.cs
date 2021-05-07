namespace ToSic.Sxc.Web.WebApi
{
    public class LinkHelpers
    {
        /**
         * Move queryString part from 'api' to 'parameters'.
         */
        public static void NormalizeQueryString(ref string api, ref string queryString)
        {
            if (api == null) return; // nothing to do
            
            // clean beginning of queryString
            if (!string.IsNullOrEmpty(queryString)) queryString = queryString.TrimStart('?').TrimStart('&');
            
            var queryStringDelimiterPosition = api.IndexOf("?");
            if (queryStringDelimiterPosition <= -1) return; // nothing to do

            queryString = (string.IsNullOrEmpty(queryString)) ? $"{api.Substring(queryStringDelimiterPosition+1)}" : $"{api.Substring(queryStringDelimiterPosition+1)}&{queryString}";

            api = api.Substring(0, queryStringDelimiterPosition);
            //if (!string.IsNullOrEmpty(queryString) && !queryString.StartsWith("?")) queryString = $"?{queryString}";
        }
    }
}
