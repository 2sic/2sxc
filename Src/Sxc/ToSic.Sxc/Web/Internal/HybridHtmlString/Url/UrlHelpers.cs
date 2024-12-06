using System.Collections.Specialized;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Web.Internal.Url;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
        query = query?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(query))
            return nvc;

        // remove anything other than query string from url
        if (query.StartsWith("?"))
            query = query.Substring(1);

        foreach (var vp in query.Split('&'))
        {
            if (string.IsNullOrWhiteSpace(vp)) continue;

            var singlePair = vp.Split('=');
            nvc.Add(singlePair[0], singlePair.Length == 2 ? singlePair[1] : string.Empty);
        }

        return nvc;
    }

    /// <summary>
    /// Converts a NameValueCollection to string.
    /// Used in link generations and especially also the <see cref="Parameters"/>
    /// so be very careful if you change anything!
    /// </summary>
    /// <param name="nvc"></param>
    /// <param name="prioritize"></param>
    /// <returns></returns>
    public static NameValueCollection Sort(this NameValueCollection nvc, string prioritize = default)
    {
        // Figure Out best key order, respecting the custom prioritization
        var customKeys = prioritize.CsvToArrayWithoutEmpty().ToList();
        var keys = customKeys.Count > 0
            ? nvc.AllKeys.OrderBy(k =>
            {
                // find index case-insensitive
                var index = customKeys.FindIndex(x => x.EqualsInsensitive(k));
                return (index != -1 ? index.ToString("000") : "999") + "-" + k;
            })
            : nvc.AllKeys.OrderBy(k => k);

        // create a new NVC but sorted
        var sorted = new NameValueCollection();
        foreach (var key in keys)
        {
            var values = nvc.GetValues(key)?.OrderBy(v => v).ToArray();
            if (values == null || values.Length == 0)
                sorted.Add(key, null);
            else
                foreach (var value in values)
                    sorted.Add(key, value);
        }
        return sorted;
    }


    /// <summary>
    /// Converts a NameValueCollection to string.
    /// Used in link generations and especially also the <see cref="Parameters"/>
    /// so be very careful if you change anything!
    /// </summary>
    /// <param name="nvc"></param>
    /// <returns></returns>
    public static string NvcToString(this NameValueCollection nvc) 
        => NvcToString(nvc, "=", "&", "", "", true, null);

    private class KeyValuePairTemp
    {
        public string Key;
        public string Value;
    }

    internal static string NvcToString(NameValueCollection nvc, string keyValueSeparator, string pairSeparator,
        string terminator, string empty, bool repeatKeyForEachValue, string valueSeparator)
    {
        // Note 2dm: reworked this entire logic 2022-04-07, all tests passed, believe it's ok, but there is a minimal risk
        var allPairs = nvc.AllKeys
            .SelectMany(key =>
            {
                var values = nvc.GetValues(key);
                var noValues = values == null || values.Length == 0;
                if (!noValues)
                {
                    values = values.Where(v => !string.IsNullOrEmpty(v)).ToArray();
                    noValues = values.Length == 0;
                }

                // Key null; 2 options left, values or no values
                if (key is null)
                    return noValues
                        ? []
                        // If no key, treat the values as standalone keys
                        : values.Select(v => new KeyValuePairTemp { Key = v, Value = null }).ToArray();

                // Key not null, no values
                if (noValues) return [new() { Key = key, Value = null }];

                // Key null, values - two options - give as array or single item
                return repeatKeyForEachValue 
                    ? values.Select(v => new KeyValuePairTemp { Key = key, Value = v.ToString() }) 
                    : [new() { Key = key, Value = string.Join(valueSeparator, values) }];
            })
            .Select(set => set.Key + (string.IsNullOrEmpty(set.Value) ? "" : keyValueSeparator + set.Value))
            //.SelectMany(set =>
            //{
            //    var key = set.Key;
            //    // Important - both key and values can be null; values can be a list of things
            //    var values = set.AllValues;// nvc.GetValues(key);
            //    var noValues = (values == null || values.Length == 0);
            //    if (key is null)
            //    {
            //        if (noValues) return Array.Empty<string>();
            //        if (repeatKeyForEachValue)
            //            return values.Select(v => v.ToString())
            //                .ToArray();
            //        return new[] { string.Join(valueSeparator, values) };
            //    }

            //    if (noValues) return new[] { key };

            //    if (repeatKeyForEachValue)
            //        return values.Select(v => key + (string.IsNullOrEmpty(v) ? "" : keyValueSeparator + v));

            //    return new[] { key + (values.All(string.IsNullOrEmpty) ? "" : keyValueSeparator + string.Join(valueSeparator, values)) };
            //})
            .ToArray();

        return allPairs.Any()
            ? string.Join(pairSeparator, allPairs) + terminator
            : empty;
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
                var values = source.GetValues(k) ?? [null]; // catch null-values

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


    public static string AddQueryString(string url, string newParams) => AddQueryString(url, ParseQueryString(newParams));

    public static string AddQueryString(string url, NameValueCollection newParams)
    {
        // check do we have any work to do
        if (newParams == null || newParams.Count == 0) return url;

        // 1. Get only the query string parts
        var parts = new UrlParts(url);

        // if the url already has some params we should take that and split it into it's pieces
        var queryParams = ParseQueryString(parts.Query);

        // new params would replace existing queryString params or append new param to queryString
        var finalParams = queryParams.Merge(newParams);

        // combine new query string in url
        return GetUrlWithUpdatedQueryString(parts, finalParams);
    }


    private static string GetUrlWithUpdatedQueryString(UrlParts parts, NameValueCollection queryString)
    {
        var newUrl = parts.ToLink(suffix: false);
        if (queryString.Count > 0)
            newUrl += UrlParts.QuerySeparator + NvcToString(queryString);

        if (!string.IsNullOrWhiteSpace(parts.Fragment))
            newUrl += UrlParts.FragmentSeparator + parts.Fragment;

        return newUrl;

    }


    public static string RemoveQuery(this string url) => RemoveAfterSeparator(url, UrlParts.QuerySeparator);
    public static string RemoveFragment(this string url) => RemoveAfterSeparator(url, UrlParts.FragmentSeparator);
    public static string RemoveQueryAndFragment(this string url) => url.RemoveQuery().RemoveFragment();
    private static string RemoveAfterSeparator(string @string, char separator)
    {
        if (string.IsNullOrEmpty(@string)) return @string;
        var start = @string.IndexOf(separator);
        return start < 0 ? @string : @string.Substring(0, start);
    }
}