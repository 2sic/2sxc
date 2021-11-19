//using System.Collections.Specialized;
//using ToSic.Sxc.Web.Url;

//namespace ToSic.Sxc.Web
//{
//    public class QueryHelper
//    {
//        public static string AddQueryString(string url, string newParams) => AddQueryString(url, UrlHelpers.ParseQueryString(newParams));

//        public static string AddQueryString(string url, NameValueCollection newParams)
//        {
//            // check do we have any work to do
//            if (newParams == null || newParams.Count == 0) return url;

//            // 1. Get only the query string parts
//            var parts = new UrlParts(url);

//            // if the url already has some params we should take that and split it into it's pieces
//            var queryParams = UrlHelpers.ParseQueryString(parts.Query);

//            // new params would replace existing queryString params or append new param to queryString
//            var finalParams = queryParams.Merge(newParams);

//            // combine new query string in url
//            return GetUrlWithUpdatedQueryString(parts, finalParams);
//        }


//        private static string GetUrlWithUpdatedQueryString(UrlParts parts, NameValueCollection queryString)
//        {
//            var newUrl = parts.ToLink(suffix: false);
//            if (queryString.Count > 0)
//                newUrl += UrlParts.QuerySeparator + UrlHelpers.NvcToString(queryString);

//            if (!string.IsNullOrWhiteSpace(parts.Fragment))
//                newUrl += UrlParts.FragmentSeparator + parts.Fragment;

//            return newUrl;
            
//        }
        
//    }
//}
