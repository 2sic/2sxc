using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data.Wrapper;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data.Typed
{
    [PrivateApi]
    [JsonConverter(typeof(DynamicJsonConverter))]
    internal class WrapObjectTyped: Wrapper<object>, ITyped, /*IPropertyLookup,*/ IHasPropLookup, IHasJsonSource
    {
        protected readonly CodeDataWrapper Wrapper;
        protected readonly PreWrapObject PreWrap;

        public WrapObjectTyped(PreWrapObject preWrap, CodeDataWrapper wrapper) : base(preWrap.GetContents())
        {
            Wrapper = wrapper;
            PreWrap = preWrap;
        }

        IPropertyLookup IHasPropLookup.PropertyLookup => PreWrap;


        dynamic ITyped.Dyn => this;

        #region Keys

        bool ITyped.ContainsKey(string name) => TypedHelpers.ContainsKey(name, this,
            (e, k) => e.PreWrap.ContainsKey(k),
            (e, k) =>
            {
                var child = e.PreWrap.TryGet(k);
                if (!child.Found || child.Result == null) return null;
                if (child.Result is WrapObjectTyped typed) return typed;
                // Note: arrays have never been supported, so the following won't work
                // Because the inner objects are not of the expected type.
                // We don't want to start supporting it now.
                // Leave this code in though, so we know that we did try it.
                //if (child.Result is IEnumerable list)
                //    return list.Cast<WrapObjectTyped>().FirstOrDefault(o => o != null);
                return null;
            }
        );

        bool ITyped.ContainsData(string name) => (this as ITyped).Get(name, required: false) != null;

        IEnumerable<string> ITyped.Keys(string noParamOrder, IEnumerable<string> only)
            => PreWrap.Keys(noParamOrder, only);

        #endregion

        object ITyped.Get(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            return PreWrap.TryGet(name, true).Result;
        }

        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback, bool? required)
            => PreWrap.TryGet(name, noParamOrder, fallback, required: required);

        bool ITyped.Bool(string name, string noParamOrder, bool fallback, bool? required)
            => PreWrap.TryGet(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback, bool? required)
            => PreWrap.TryGet(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        string ITyped.String(string name, string noParamOrder, string fallback, bool? required, object scrubHtml)
        {
            var value = PreWrap.TryGet(name, noParamOrder: noParamOrder, fallback: fallback, required: required);
            return TypedItemHelpers.MaybeScrub(value, scrubHtml, () => Wrapper.Cdf.Value.Services.Scrub);
//#pragma warning disable CS0618
//            return scrubHtml ? Tags.Strip(value) : value;
//#pragma warning restore CS0618
        }

        int ITyped.Int(string name, string noParamOrder, int fallback, bool? required)
            => PreWrap.TryGet(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        long ITyped.Long(string name, string noParamOrder, long fallback, bool? required)
            => PreWrap.TryGet(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        float ITyped.Float(string name, string noParamOrder, float fallback, bool? required)
            => PreWrap.TryGet(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback, bool? required)
            => PreWrap.TryGet(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        double ITyped.Double(string name, string noParamOrder, double fallback, bool? required)
            => PreWrap.TryGet(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        string ITyped.Url(string name, string noParamOrder, string fallback, bool? required)
        {
            var url = PreWrap.TryGet(name, noParamOrder: noParamOrder, fallback, required: required);
            return Tags.SafeUrl(url).ToString();
        }

        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback, bool? required)
        {
            Protect(noParamOrder, nameof(fallback));
            var value = PreWrap.TryGet(name, false).Result;
            var strValue = Wrapper.ConvertForCode.ForCode(value, fallback: fallback);
            return strValue is null ? null : new RawHtmlString(WebUtility.HtmlEncode(strValue));
        }

        #region Explicit interfaces for Json, PropertyLookup etc.

        [PrivateApi]
        object IHasJsonSource.JsonSource
            => PreWrap.JsonSource;

        //[PrivateApi]
        //PropReqResult IPropertyLookup.FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path) 
        //    => PreWrap.FindPropertyInternal(specs, path);

        //[PrivateApi]
        //List<PropertyDumpItem> IPropertyLookup._Dump(PropReqSpecs specs, string path) 
        //    => PreWrap._Dump(specs, path);

        #endregion
    }
}
