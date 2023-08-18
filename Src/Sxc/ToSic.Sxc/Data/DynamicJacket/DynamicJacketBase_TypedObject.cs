//using System;
//using System.Collections.Generic;
//using System.Net;
//using ToSic.Lib.Documentation;
//using ToSic.Razor.Blade;
//using ToSic.Razor.Markup;
//using ToSic.Sxc.Data.Typed;
//using static ToSic.Eav.Parameters;

//namespace ToSic.Sxc.Data
//{
//    public abstract partial class DynamicJacketBase: ITyped
//    {
//        [PrivateApi]
//        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback, bool? required)
//        {
//            Protect(noParamOrder, nameof(fallback));
//            var value = PreWrap.TryGetWrap(name).Result;
//            var strValue = Wrapper.ConvertForCode.ForCode(value, fallback: fallback);
//            return strValue is null ? null : new RawHtmlString(WebUtility.HtmlEncode(strValue));
//        }

//        [PrivateApi]
//        public bool ContainsKey(string name) => PreWrap.ContainsKey(name);

//        [PrivateApi]
//        public bool IsEmpty(string name, string noParamOrder = Protector)//, bool? blankIs = default) 
//            => HasKeysHelper.IsEmpty(this, name, noParamOrder, default /*blankIs*/);

//        [PrivateApi]
//        public bool IsNotEmpty(string name, string noParamOrder = Protector)//, bool? blankIs = default) 
//            => HasKeysHelper.IsNotEmpty(this, name, noParamOrder, default /*blankIs*/);

//        [PrivateApi]
//        public IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default)
//            => PreWrap.Keys(noParamOrder, only);

//        [PrivateApi]
//        object ITyped.Get(string name, string noParamOrder, bool? required)
//        {
//            Protect(noParamOrder, nameof(required));
//            return PreWrap.TryGetWrap(name).Result;
//        }

//        public TValue Get<TValue>(string name, string noParamOrder = Protector, TValue fallback = default) 
//            => PreWrap.TryGetTyped(name, noParamOrder, fallback, false);

//        [PrivateApi]
//        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback, bool? required) 
//            => PreWrap.TryGetTyped(name, noParamOrder, fallback, required);

//        [PrivateApi]
//        dynamic ITyped.Dyn => this;

//        [PrivateApi] 
//        bool ITyped.Bool(string name, string noParamOrder, bool fallback, bool? required)
//            => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required);

//        [PrivateApi]
//        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback, bool? required)
//            => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required);

//        [PrivateApi]
//        string ITyped.String(string name, string noParamOrder, string fallback, bool? required, object scrubHtml)
//        {
//            var value = PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required);
//            return TypedItemHelpers.MaybeScrub(value, scrubHtml, () => Wrapper.Cdf.Value.Services.Scrub);
//        }

//        [PrivateApi]
//        int ITyped.Int(string name, string noParamOrder, int fallback, bool? required) 
//            => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required);

//        [PrivateApi]
//        long ITyped.Long(string name, string noParamOrder, long fallback, bool? required) 
//            => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required);

//        [PrivateApi]
//        float ITyped.Float(string name, string noParamOrder, float fallback, bool? required)
//            => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required);

//        [PrivateApi]
//        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback, bool? required)
//            => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required);

//        [PrivateApi]
//        double ITyped.Double(string name, string noParamOrder, double fallback, bool? required)
//            => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required);

//        [PrivateApi]
//        string ITyped.Url(string name, string noParamOrder, string fallback, bool? required)
//        {
//            var url = PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required);
//            return Tags.SafeUrl(url).ToString();
//        }
//    }
//}
