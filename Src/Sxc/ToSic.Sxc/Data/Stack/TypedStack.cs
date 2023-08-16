using System;
using System.Collections.Generic;
using System.Linq;
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
    [PrivateApi]
    internal class TypedStack: IWrapper<IPropertyStack>, ITypedStack, IHasPropLookup, ICanDebug, ICanGetByName
    {
        public TypedStack(string name, CodeDataFactory cdf, IReadOnlyCollection<KeyValuePair<string, IPropertyLookup>> sources)
        {
            _stack = new PropertyStack().Init(name, sources);
            Cdf = cdf;
            PropertyLookup = new PropLookupStack(_stack, () => Debug);
            _helper = new GetAndConvertHelper(this, cdf, strict: false, childrenShouldBeDynamic: false, canDebug: this);
            _itemHelper = new CodeItemHelper(_helper, this);
        }

        private readonly IPropertyStack _stack;
        [PrivateApi]
        public IPropertyLookup PropertyLookup { get; }
        private readonly GetAndConvertHelper _helper;
        private readonly CodeItemHelper _itemHelper;

        public IPropertyStack GetContents() => _stack;

        public CodeDataFactory Cdf { get; }

        public bool Debug { get; set; }

        #region GetByName - to allow this to be used for image settings etc.

        object ICanGetByName.Get(string name) => (this as ITyped).Get(name);

        #endregion


        #region ITyped.Keys and Dyn - both not implemented

        [PrivateApi]
        bool ITyped.ContainsKey(string name) 
            => throw new NotImplementedException($"Not yet implemented on {nameof(ITypedStack)}");

        bool ITyped.ContainsData(string name) => _itemHelper.ContainsData(name);

        // TODO: Keys()
        public IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default) 
            => throw new NotImplementedException();

        [PrivateApi]
        dynamic ITyped.Dyn 
            => throw new NotSupportedException($"{nameof(ITyped.Dyn)} is not supported on the {nameof(ITypedStack)} by design");

        #endregion

        #region ITyped

        [PrivateApi]
        object ITyped.Get(string name, string noParamOrder, bool? required)
            => _itemHelper.Get(name, noParamOrder, required);

        [PrivateApi]
        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback, bool? required)
            => _itemHelper.G4T(name, noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback, bool? required)
            => _itemHelper.Attribute(name, noParamOrder, fallback, required);


        [PrivateApi]
        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback, bool? required)
            => _itemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        string ITyped.String(string name, string noParamOrder, string fallback, bool? required, object scrubHtml)
            => _itemHelper.String(name, noParamOrder, fallback, required, scrubHtml);

        [PrivateApi]
        int ITyped.Int(string name, string noParamOrder, int fallback, bool? required)
            => _itemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        bool ITyped.Bool(string name, string noParamOrder, bool fallback, bool? required)
            => _itemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        long ITyped.Long(string name, string noParamOrder, long fallback, bool? required)
            => _itemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        float ITyped.Float(string name, string noParamOrder, float fallback, bool? required)
            => _itemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback, bool? required)
            => _itemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        double ITyped.Double(string name, string noParamOrder, double fallback, bool? required)
            => _itemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

        [PrivateApi]
        string ITyped.Url(string name, string noParamOrder, string fallback, bool? required)
            => _itemHelper.Url(name, noParamOrder, fallback, required);

        [PrivateApi]
        string ITyped.ToString() => "test / debug: " + ToString();



        #endregion

        #region Add-Ons for ITypedStack

        ITypedItem ITypedStack.Child(string name, string noParamOrder, bool? required)
        {
            var findResult = _helper.TryGet(name);
            return IsErrStrict(findResult.Found, required, _helper.StrictGet)
                ? throw ErrStrict(name)
                : Cdf.AsItem(findResult.Result, noParamOrder);
        }

        IEnumerable<ITypedItem> ITypedStack.Children(string field, string noParamOrder, string type, bool? required)
        {
            var findResult = _helper.TryGet(field);
            return IsErrStrict(findResult.Found, required, _helper.StrictGet)
                ? throw ErrStrict(field)
                : Cdf.AsItems(findResult.Result, noParamOrder)
                    // Apply type filter - even if a bit "late"
                    .Where(i => i.Entity.Type.Is(type))
                    .ToList();
        }

        #endregion
    }
}
