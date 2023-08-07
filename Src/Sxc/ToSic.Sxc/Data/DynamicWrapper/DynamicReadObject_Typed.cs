using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data.Typed;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial class DynamicReadObject: ITyped //, IGet4Typed //, IHasKeys
    {
        [PrivateApi]
        dynamic ITyped.Dyn => this;

        [PrivateApi]
        bool ITyped.ContainsKey(string name) => Analyzer.ContainsKey(name); // _ignoreCaseLookup.ContainsKey(name);

        //IEnumerable<string> IHasKeys.Keys(string noParamOrder, IEnumerable<string> only)
        //    => TypedHelpers.FilterKeysIfPossible(noParamOrder, only, _ignoreCaseLookup?.Keys);

        //bool IHasKeys.ContainsKey(string name) => _ignoreCaseLookup.ContainsKey(name);

        [PrivateApi]
        IEnumerable<string> ITyped.Keys(string noParamOrder, IEnumerable<string> only)
            => Analyzer.Keys(noParamOrder, only); // TypedHelpers.FilterKeysIfPossible(noParamOrder, only, _ignoreCaseLookup?.Keys);

        [PrivateApi]
        object ITyped.Get(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            return Analyzer.TryGet(name).Result;
        }

        [PrivateApi]
        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback, bool? required)
            => G4T(name, noParamOrder, fallback);

        //(bool Found, object Result) IGet4Typed.Get(string name)
        //{
        //    var result = _analyzer.FindValueOrNull(name);
        //    return (result != null, result);
        //}

        [PrivateApi]
        private TValue G4T<TValue>(string name, string noParamOrder, TValue fallback, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(fallback), methodName: cName);
            return Analyzer.TryGet(name).Result.ConvertOrFallback(fallback);
        }

        //TValue IGet4Typed.G4T<TValue>(string name, string noParamOrder, TValue fallback, string cName)
        //{
        //    return G4T(name, noParamOrder, fallback, cName);
        //}


        [PrivateApi]
        bool ITyped.Bool(string name, string noParamOrder, bool fallback, bool? required)
            => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback, bool? required)
            => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        string ITyped.String(string name, string noParamOrder, string fallback, bool? required, bool scrubHtml)
        {
            var value = G4T(name, noParamOrder: noParamOrder, fallback: fallback);
#pragma warning disable CS0618
            return scrubHtml ? Razor.Blade.Tags.Strip(value) : value;
#pragma warning restore CS0618

        }

        [PrivateApi]
        int ITyped.Int(string name, string noParamOrder, int fallback, bool? required)
            => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        long ITyped.Long(string name, string noParamOrder, long fallback, bool? required)
            => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        float ITyped.Float(string name, string noParamOrder, float fallback, bool? required)
            => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback, bool? required)
            => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        double ITyped.Double(string name, string noParamOrder, double fallback, bool? required)
            => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        string ITyped.Url(string name, string noParamOrder, string fallback, bool? required)
        {
            var url = G4T(name, noParamOrder: noParamOrder, fallback);
            return Tags.SafeUrl(url).ToString();
        }

        // 2023-07-31 turned off again as not final and probably not a good idea #ITypedIndexer
        //[PrivateApi]
        //IRawHtmlString ITyped.this[string name] => new TypedItemValue(Get(name));


        //TValue ITyped.Get<TValue>(string name) => GetV<TValue>(name, Protector, default);


        [PrivateApi]
        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback, bool? required)
        {
            Protect(noParamOrder, nameof(fallback));
            var value = Analyzer.TryGet(name).Result;
            var strValue = WrapperFactory.ConvertForCode.ForCode(value, fallback: fallback);
            return strValue is null ? null : new RawHtmlString(WebUtility.HtmlEncode(strValue));
        }
    }
}
