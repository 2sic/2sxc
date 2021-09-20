using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace ToSic.Sxc.Web
{
    public class QueryHelper
    {
        public static string AddQueryString(string url, List<KeyValuePair<string, string>> queryParams)
        {
            // check do we have any work to do
            if (queryParams.Count == 0) return url;

            // Problem ATM:
            // - if the url is something like "test.jpg?w=200" then running this function gets you something like test.jpg?w=200?w=1600
            // We should try to combine existing params - so if the url already has a "?..." we should merge these
            // Basically the logic we need is
            // - if the url already has some params we should take that and split it into it's pieces
            // ...ideally using some .net url processing API and not invent our own
            // Then we should check if our new params (the `queryString` dictionary would replace any of the params
            // ...if yes, they should be removed
            // then everything should be re-assembled to work

            // prepare temp absolute uri because it is required for ParseQueryString
            var tempAbsoluteUri = GetTempAbsoluteUri(url);

            // if the url already has some params we should take that and split it into it's pieces
            var queryString = HttpUtility.ParseQueryString(tempAbsoluteUri.Query);

            // new params would update existing queryString params or append new param to queryString
            queryParams.ForEach(param => queryString.Set(param.Key, param.Value));

            // combine new query string in url
            return GetUrlWithUpdatedQueryString(url, tempAbsoluteUri, queryString);
        }

        private static Uri GetTempAbsoluteUri(string url)
        {
            var isAbsoluteWithoutProtocol = url.StartsWith(@"//");
            var isAbsoluteUrl = url.StartsWith(@"http") || isAbsoluteWithoutProtocol;

            // special handling for relative urls, because of ParseQueryString limitation (it is working only with absolute uris)
            var absoluteUri = isAbsoluteUrl
                ? new Uri(isAbsoluteWithoutProtocol ? "http:" + url : url, UriKind.Absolute)
                : new Uri(new Uri("http://unknown/", UriKind.Absolute), url); // generate temp/dummy absolute uri, just for use with ParseQueryString

            return absoluteUri;
        }

        private static string GetUrlWithUpdatedQueryString(string url, Uri tempUri, NameValueCollection queryString)
        {
            var newQueryString = "?" + queryString;

            // check for old query string to replace
            if (string.IsNullOrEmpty(tempUri.Query))
                // check for #fragment to handle
                if (string.IsNullOrEmpty(tempUri.Fragment))
                    url += newQueryString;
                else // #fragment should be on the end or url
                    url = url.Replace(tempUri.Fragment, newQueryString + tempUri.Fragment);
            else // replace old query string with new one
                url = url.Replace(tempUri.Query, newQueryString);

            return url;
        }
    }
}
