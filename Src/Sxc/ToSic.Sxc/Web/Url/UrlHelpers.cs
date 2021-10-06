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
        /// Used in link generations and especially also the <see cref="ToSic.Sxc.Context.Query.Parameters"/>
        /// so be very careful if you change anything!
        /// </summary>
        /// <param name="nvc"></param>
        /// <returns></returns>
        public static string NvcToString(NameValueCollection nvc)
        {
            var allPairs = nvc.AllKeys
                .Where(k => !string.IsNullOrEmpty(k))
                .SelectMany(k =>
                {
                    var vals = nvc.GetValues(k) ?? new[] { "" }; // catch null-values
                    return vals.Select(v => k + (string.IsNullOrEmpty(v) ? "" : "=" + v));
                })
                .ToArray();
            return allPairs.Any() ? string.Join("&", allPairs) : "";
        }

    }




}
