using System.Collections.Specialized;
using System.Linq;

namespace ToSic.Sxc.Web.Url
{
    public static class UrlHelpers
    {
        /// <summary>
        /// Safer replacement to the HttpUtility.ParseQueryString because that changes umlauts etc. to %u0043 characters which is not very common
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <remarks>
        /// See https://stackoverflow.com/questions/68624/how-to-parse-a-query-string-into-a-namevaluecollection-in-net
        /// </remarks>
        public static NameValueCollection ParseQueryString(string query)
        {
            // Note that this NameValueCollection is different than the one from HttpUtility
            // but because that one is internal, we cannot create one directly
            var nvc = new NameValueCollection();
            query = (query ?? "").Trim();
            if (string.IsNullOrWhiteSpace(query)) return nvc;

            // remove anything other than query string from url
            if (query.StartsWith("?")) query = query.Substring(1);

            foreach (var vp in query.Split('&'))
            {
                if(string.IsNullOrWhiteSpace(vp)) continue;

                var singlePair = vp.Split('=');
                nvc.Add(singlePair[0], singlePair.Length == 2 ? singlePair[1] : string.Empty);
            }

            return nvc;
        }

        /// <summary>
        /// Converts a NameValueCollection to string.
        /// Used in link generations and especially also the <see cref="Context.Query.Parameters"/>
        /// so be very careful if you change anything!
        /// </summary>
        /// <param name="nvc"></param>
        /// <returns></returns>
        public static string NvcToString(NameValueCollection nvc)
        {
            var allPairs = nvc.AllKeys
                .SelectMany(key =>
                {
                    // Important - both key and values can be null; values can be a list of things
                    var values = nvc.GetValues(key);
                    var noValues = (values == null || values.Length == 0);
                    if (key is null)
                        return noValues
                            ? new string[0] // No keys or values, empty list
                            : values.Select(v => v.ToString()).ToArray(); // in case values are without keys, join them like this

                    return noValues
                        ? new[] { key }
                        : values.Select(v => key + (string.IsNullOrEmpty(v) ? "" : "=" + v));                        
                })
                .ToArray();
            return allPairs.Any() ? string.Join("&", allPairs) : "";
        }


        /// <summary>
        /// Import an NVC into another
        /// </summary>
        /// <returns></returns>
        public static NameValueCollection Merge(this NameValueCollection first, NameValueCollection source, bool replace = false)
        {
            var target = new NameValueCollection(first);
            source.AllKeys
                .Where(k => !string.IsNullOrEmpty(k))
                .ToList()
                .ForEach(k =>
                {
                    var values = source.GetValues(k) ?? new string[] { null }; // catch null-values

                    foreach (var v in values)
                    {
                        if (replace)
                            target.Set(k, v);
                        else
                            target.Add(k, v);
                    }
                });
            return target;
        }


        public static string QuickAddUrlParameter(string url, string name, string value) 
            => $"{url}{(url.IndexOf('?') > 0 ? '&' : '?')}{name}={value}";


        public static string AddQueryString(string url, string newParams) => AddQueryString(url, UrlHelpers.ParseQueryString(newParams));

        public static string AddQueryString(string url, NameValueCollection newParams)
        {
            // check do we have any work to do
            if (newParams == null || newParams.Count == 0) return url;

            // 1. Get only the query string parts
            var parts = new UrlParts(url);

            // if the url already has some params we should take that and split it into it's pieces
            var queryParams = UrlHelpers.ParseQueryString(parts.Query);

            // new params would replace existing queryString params or append new param to queryString
            var finalParams = queryParams.Merge(newParams);

            // combine new query string in url
            return GetUrlWithUpdatedQueryString(parts, finalParams);
        }


        private static string GetUrlWithUpdatedQueryString(UrlParts parts, NameValueCollection queryString)
        {
            var newUrl = parts.ToLink(suffix: false);
            if (queryString.Count > 0)
                newUrl += UrlParts.QuerySeparator + UrlHelpers.NvcToString(queryString);

            if (!string.IsNullOrWhiteSpace(parts.Fragment))
                newUrl += UrlParts.FragmentSeparator + parts.Fragment;

            return newUrl;

        }
    }




}
