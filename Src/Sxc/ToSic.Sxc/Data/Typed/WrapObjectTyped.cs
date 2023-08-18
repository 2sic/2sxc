using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Data;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data.Wrapper;
using ToSic.Sxc.Services;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data.Typed
{
    [PrivateApi]
    [JsonConverter(typeof(DynamicJsonConverter))]
    public class WrapObjectTyped: IWrapper<IPreWrap>, ITyped, IHasPropLookup, IHasJsonSource
    {
        private readonly LazySvc<IScrub> _scrubSvc;
        private readonly LazySvc<ConvertForCodeService> _forCodeConverter;
        internal IPreWrap PreWrap { get; private set; }

        public WrapObjectTyped(LazySvc<IScrub> scrubSvc, LazySvc<ConvertForCodeService> forCodeConverter)
        {
            _scrubSvc = scrubSvc;
            _forCodeConverter = forCodeConverter;
        }

        internal WrapObjectTyped Setup(IPreWrap preWrap)
        {
            PreWrap = preWrap;
            return this;
        }

        public IPreWrap GetContents() => PreWrap;

        IPropertyLookup IHasPropLookup.PropertyLookup => PreWrap;

        #region Keys

        public bool ContainsKey(string name) => TypedHelpers.ContainsKey(name, this,
            (e, k) => e.PreWrap.ContainsKey(k),
            (e, k) =>
            {
                var child = e.PreWrap.TryGetWrap(k);
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

        public bool IsEmpty(string name, string noParamOrder = Protector)//, bool? blankIs = default)
            => HasKeysHelper.IsEmpty(this, name, noParamOrder, default /*blankIs*/);

        public bool IsNotEmpty(string name, string noParamOrder = Protector)//, bool? blankIs = default)
            => HasKeysHelper.IsNotEmpty(this, name, noParamOrder, default /*blankIs*/);



        public IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default)
            => PreWrap.Keys(noParamOrder, only);

        #endregion

        object ITyped.Get(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            return PreWrap.TryGetObject(name, noParamOrder, required);
        }

        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback, bool? required)
            => PreWrap.TryGetTyped(name, noParamOrder, fallback, required: required);

        bool ITyped.Bool(string name, string noParamOrder, bool fallback, bool? required)
            => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback, bool? required)
            => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        string ITyped.String(string name, string noParamOrder, string fallback, bool? required, object scrubHtml)
        {
            var value = PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);
            return TypedItemHelpers.MaybeScrub(value, scrubHtml, () => _scrubSvc.Value);
        }

        int ITyped.Int(string name, string noParamOrder, int fallback, bool? required)
            => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        long ITyped.Long(string name, string noParamOrder, long fallback, bool? required)
            => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        float ITyped.Float(string name, string noParamOrder, float fallback, bool? required)
            => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback, bool? required)
            => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        double ITyped.Double(string name, string noParamOrder, double fallback, bool? required)
            => PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        string ITyped.Url(string name, string noParamOrder, string fallback, bool? required)
        {
            var url = PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback, required: required);
            return Tags.SafeUrl(url).ToString();
        }

        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback, bool? required)
        {
            Protect(noParamOrder, nameof(fallback));
            var value = PreWrap.TryGetWrap(name, false).Result;
            var strValue = _forCodeConverter.Value.ForCode(value, fallback: fallback);
            return strValue is null ? null : new RawHtmlString(WebUtility.HtmlEncode(strValue));
        }

        #region Explicit interfaces for Json, PropertyLookup etc.

        [PrivateApi]
        object IHasJsonSource.JsonSource
            => PreWrap.JsonSource;

        #endregion

    }
}
