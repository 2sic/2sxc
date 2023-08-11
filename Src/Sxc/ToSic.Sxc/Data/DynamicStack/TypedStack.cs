using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Razor.Markup;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    internal class TypedStack: Wrapper<IPropertyStack>, ITypedStack, ICanDebug
    {
        public TypedStack(string name, CodeDataFactory cdf, IReadOnlyCollection<KeyValuePair<string, IPropertyLookup>> sources)
        : base(new PropertyStack().Init(name, sources))
        {
            Cdf = cdf;
        }

        private PreWrapStack PreWrap => _preWrap ?? (_preWrap = new PreWrapStack(GetContents(), () => Debug));
        private PreWrapStack _preWrap;
        [PrivateApi]
        // ReSharper disable once InconsistentNaming
        public CodeDataFactory Cdf { get; }


        [PrivateApi]
        internal CodeEntityHelper Helper => _helper ?? (_helper = new CodeEntityHelper(PreWrap, Cdf, strict: false, () => Debug));
        private CodeEntityHelper _helper;

        public bool Debug { get; set; }

        [PrivateApi]
        internal CodeItemHelper ItemHelper => _itemHelper ?? (_itemHelper = new CodeItemHelper(Helper));
        private CodeItemHelper _itemHelper;



        #region ITyped.Keys

        [PrivateApi]
        bool ITyped.ContainsKey(string name)
        {
            //return UnwrappedStack.Sources.Any(s =>
            //{
            //    switch (s.Value)
            //    {
            //        case null:
            //            return false;
            //        case ITyped typed:
            //            return typed.ContainsKey(name);
            //        case IHasKeys keyed:
            //            return keyed.ContainsKey(name);
            //    }

            //    return false;
            //});
            throw new NotImplementedException($"Not yet implemented on {nameof(ITypedStack)}");
        }

        // TODO: Keys()
        public IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region ITyped


        [PrivateApi]
        object ITyped.Get(string name, string noParamOrder, bool? required)
            => ItemHelper.Get(name, noParamOrder, required);

        [PrivateApi]
        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback, bool? required)
            => ItemHelper.Attribute(name, noParamOrder, fallback, required);

        [PrivateApi]
        dynamic ITyped.Dyn => throw new NotSupportedException($"{nameof(ITyped.Dyn)} is not supported on the {nameof(ITypedStack)} by design");


        [PrivateApi]
        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        string ITyped.String(string name, string noParamOrder, string fallback, bool? required, bool scrubHtml)
            => ItemHelper.String(name, noParamOrder, fallback, required, scrubHtml);

        [PrivateApi]
        int ITyped.Int(string name, string noParamOrder, int fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        bool ITyped.Bool(string name, string noParamOrder, bool fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        long ITyped.Long(string name, string noParamOrder, long fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        float ITyped.Float(string name, string noParamOrder, float fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        double ITyped.Double(string name, string noParamOrder, double fallback, bool? required)
            => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        string ITyped.Url(string name, string noParamOrder, string fallback, bool? required)
            => ItemHelper.Url(name, noParamOrder, fallback, required);

        [PrivateApi]
        string ITyped.ToString() => "test / debug: " + ToString();

        #endregion

        #region Add-Ons for ITypedStack

        ITypedItem ITypedStack.Child(string name, string noParamOrder, bool? required)
        {
            var findResult = Helper.TryGet(name);
            return IsErrStrict(findResult.Found, required, Helper.StrictGet)
                ? throw ErrStrict(name)
                : Cdf.AsItem(findResult.Result, noParamOrder);
        }

        IEnumerable<ITypedItem> ITypedStack.Children(string field, string noParamOrder, string type, bool? required)
        {
            // TODO: @2DM - type-filter of children is not applied
            var findResult = Helper.TryGet(field);
            return IsErrStrict(findResult.Found, required, Helper.StrictGet)
                ? throw ErrStrict(field)
                : Cdf.AsItems(findResult.Result, noParamOrder);
        }

        #endregion
    }
}
