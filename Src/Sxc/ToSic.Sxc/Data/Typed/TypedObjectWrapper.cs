using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data.Typed
{
    [PrivateApi]
    [JsonConverter(typeof(DynamicJsonConverter))]
    internal class TypedObjectWrapper: Wrapper<object>, ITyped, IPropertyLookup, IHasJsonSource
    {
        private readonly DynamicWrapperFactory _wrapperFactory;
        private readonly Wrapper.AnalyzeObject _analyzer;

        public TypedObjectWrapper(Wrapper.AnalyzeObject analyzer, DynamicWrapperFactory wrapperFactory) : base(analyzer.GetContents())
        {
            _wrapperFactory = wrapperFactory;
            _analyzer = analyzer;
        }

        dynamic ITyped.Dyn => this;

        bool ITyped.ContainsKey(string name) => _analyzer.ContainsKey(name);

        IEnumerable<string> ITyped.Keys(string noParamOrder, IEnumerable<string> only)
            => _analyzer.Keys(noParamOrder, only);

        object ITyped.Get(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            return _analyzer.TryGet(name, true).Result;
        }

        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback, bool? required)
            => _analyzer.G4T(name, noParamOrder, fallback);

        bool ITyped.Bool(string name, string noParamOrder, bool fallback, bool? required)
            => _analyzer.G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback, bool? required)
            => _analyzer.G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        string ITyped.String(string name, string noParamOrder, string fallback, bool? required, bool scrubHtml)
        {
            var value = _analyzer.G4T(name, noParamOrder: noParamOrder, fallback: fallback);
#pragma warning disable CS0618
            return scrubHtml ? Tags.Strip(value) : value;
#pragma warning restore CS0618

        }

        int ITyped.Int(string name, string noParamOrder, int fallback, bool? required)
            => _analyzer.G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        long ITyped.Long(string name, string noParamOrder, long fallback, bool? required)
            => _analyzer.G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        float ITyped.Float(string name, string noParamOrder, float fallback, bool? required)
            => _analyzer.G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback, bool? required)
            => _analyzer.G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        double ITyped.Double(string name, string noParamOrder, double fallback, bool? required)
            => _analyzer.G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        string ITyped.Url(string name, string noParamOrder, string fallback, bool? required)
        {
            var url = _analyzer.G4T(name, noParamOrder: noParamOrder, fallback);
            return Tags.SafeUrl(url).ToString();
        }

        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback, bool? required)
        {
            Protect(noParamOrder, nameof(fallback));
            var value = _analyzer.TryGet(name, false).Result;
            var strValue = _wrapperFactory.ConvertForCode.ForCode(value, fallback: fallback);
            return strValue is null ? null : new RawHtmlString(WebUtility.HtmlEncode(strValue));
        }

        #region Explicit interfaces for Json, PropertyLookup etc.

        [PrivateApi]
        object IHasJsonSource.JsonSource
            => _analyzer.JsonSource;

        [PrivateApi]
        PropReqResult IPropertyLookup.FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path) 
            => _analyzer.FindPropertyInternal(specs, path);

        [PrivateApi]
        List<PropertyDumpItem> IPropertyLookup._Dump(PropReqSpecs specs, string path) 
            => _analyzer._Dump(specs, path);

        #endregion
    }
}
