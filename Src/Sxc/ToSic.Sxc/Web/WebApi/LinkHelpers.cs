namespace ToSic.Sxc.Web.WebApi
{
    public class LinkHelpers
    {
        /**
         * Combine api with query string.
         */
        public static string CombineApiWithQueryString(string api, string queryString)
        {
            queryString = queryString?.TrimStart('?').TrimStart('&');

            // combine api with query string
            return string.IsNullOrEmpty(queryString) ? api :
                api?.IndexOf("?") > 0 ? $"{api}&{queryString}" : $"{api}?{queryString}";
        }
    }
}
