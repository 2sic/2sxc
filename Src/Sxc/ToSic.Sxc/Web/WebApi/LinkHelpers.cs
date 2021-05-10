namespace ToSic.Sxc.Web.WebApi
{
    public class LinkHelpers
    {
        /**
         * Move queryString part from 'api' to 'parameters'.
         */
        public static string NormalizeQueryString(string api, string queryString)
        {
            if (api == null) return queryString; // nothing to do

            // clean beginning of queryString
            if (!string.IsNullOrEmpty(queryString)) queryString = queryString.TrimStart('?').TrimStart('&');

            var queryStringDelimiterPosition = api.IndexOf("?");
            if (queryStringDelimiterPosition <= -1) return queryString; // nothing to do

            return (string.IsNullOrEmpty(queryString))
                ? $"{api.Substring(queryStringDelimiterPosition + 1)}"
                : $"{api.Substring(queryStringDelimiterPosition + 1)}&{queryString}";
        }

        /**
         * Remove queryString part from 'api'.
         */
        public static string RemoveQueryString(string api)
        {
            if (api == null) return api; // nothing to do

            var queryStringDelimiterPosition = api.IndexOf("?");
            return queryStringDelimiterPosition <= -1 ? api : api.Substring(0, queryStringDelimiterPosition);
        }
    }
}
