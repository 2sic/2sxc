using System.Collections.Specialized;
using ToSic.Sxc.Context;
using ToSic.Sxc.Web.Internal.Url;
using Parameters = ToSic.Sxc.Context.Internal.Parameters;

namespace ToSic.Sxc.Tests.LinksAndImages
{
    internal static class ParametersTestExtensions
    {
        public static IParameters NewParameters(NameValueCollection originals) => new Parameters { Nvc = originals };

        public static IParameters AsParameters(this NameValueCollection originals) => new Parameters { Nvc = originals };

        public static IParameters AsParameters(this string originals) => UrlHelpers.ParseQueryString(originals).AsParameters();

        public static IParameters TestAdd(this IParameters p, string key) => p.Add(key);

        public static IParameters TestAdd(this IParameters p, string key, string value) => p.Add(key, value);

        public static IParameters TestAdd(this IParameters p, string key, object value) => p.Add(key, value);

        public static IParameters TestRemove(this IParameters p, string name) => p.Remove(name);

        public static IParameters TestRemove(this IParameters p, string name, object value) => p.Remove(name, value);

        public static IParameters TestSet(this IParameters p, string name) => p.Set(name);
        public static IParameters TestSet(this IParameters p, string name, string value) => p.Set(name, value);

        public static IParameters TestSet(this IParameters p, string name, object value) => p.Set(name, value);

        public static IParameters TestToggle(this IParameters p, string name, object value) => p.Toggle(name, value);
        public static IParameters TestFilter(this IParameters p, string names) => p.Filter(names);
    }
}
