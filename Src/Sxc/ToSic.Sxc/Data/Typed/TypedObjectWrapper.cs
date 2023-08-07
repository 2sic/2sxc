using System;
using System.Collections.Generic;
using System.Net;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data.Typed
{
    internal class TypedObjectWrapper: Wrapper<IGet4Typed>, ITyped, IHasJsonSource
    {
        public DynamicWrapperFactory WrapperFactory { get; }
        private readonly IGet4Typed _original;

        public TypedObjectWrapper(IGet4Typed original, DynamicWrapperFactory wrapperFactory) : base(original)
        {
            WrapperFactory = wrapperFactory;
            _original = original;
        }

        [PrivateApi]
        dynamic ITyped.Dyn => this;

        [PrivateApi]
        bool ITyped.ContainsKey(string name) => _original.ContainsKey(name);// _ignoreCaseLookup.ContainsKey(name);

        [PrivateApi]
        IEnumerable<string> ITyped.Keys(string noParamOrder, IEnumerable<string> only)
            => _original.Keys(noParamOrder, only);

        [PrivateApi]
        object ITyped.Get(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            return _original.Get(name).Result; //, noParamOrder)  FindValueOrNull(name);
        }

        [PrivateApi]
        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback, bool? required)
            => _original.G4T(name, noParamOrder, fallback);

        [PrivateApi]
        bool ITyped.Bool(string name, string noParamOrder, bool fallback, bool? required)
            => _original.G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback, bool? required)
            => _original.G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        string ITyped.String(string name, string noParamOrder, string fallback, bool? required, bool scrubHtml)
        {
            var value = _original.G4T(name, noParamOrder: noParamOrder, fallback: fallback);
#pragma warning disable CS0618
            return scrubHtml ? Razor.Blade.Tags.Strip(value) : value;
#pragma warning restore CS0618

        }

        [PrivateApi]
        int ITyped.Int(string name, string noParamOrder, int fallback, bool? required)
            => _original.G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        long ITyped.Long(string name, string noParamOrder, long fallback, bool? required)
            => _original.G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        float ITyped.Float(string name, string noParamOrder, float fallback, bool? required)
            => _original.G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback, bool? required)
            => _original.G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        double ITyped.Double(string name, string noParamOrder, double fallback, bool? required)
            => _original.G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        string ITyped.Url(string name, string noParamOrder, string fallback, bool? required)
        {
            var url = _original.G4T(name, noParamOrder: noParamOrder, fallback);
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
            var value = _original.Get(name).Result;
            var strValue = WrapperFactory.ConvertForCode.ForCode(value, fallback: fallback);
            return strValue is null ? null : new RawHtmlString(WebUtility.HtmlEncode(strValue));
        }

        object IHasJsonSource.JsonSource => _original.JsonSource;
    }
}
