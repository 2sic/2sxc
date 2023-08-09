using System;
using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data.Typed;

namespace ToSic.Sxc.Data
{
    public partial class DynamicStack: ITypedStack
    {
        [PrivateApi]
        bool ITyped.ContainsKey(string name)
        {
            throw new NotImplementedException($"Not yet implemented on {nameof(ITypedStack)}");
        }

        ITypedItem ITypedStack.Child(string name, string noParamOrder, bool? required)
        {
            var findResult = GetInternal(name, lookup: false);
            return TypedHelpers.IsErrStrict(findResult.Found, required, StrictGet)
                ? throw TypedHelpers.ErrStrict(name)
                : _Cdf.AsItem(findResult.Result, noParamOrder);
        }

        IEnumerable<ITypedItem> ITypedStack.Children(string field, string noParamOrder, string type, bool? required)
        {
            // TODO: @2DM - type-filter of children is not applied
            var findResult = GetInternal(field, lookup: false);
            return TypedHelpers.IsErrStrict(findResult.Found, required, StrictGet)
                ? throw TypedHelpers.ErrStrict(field)
                : _Cdf.AsItems(findResult.Result, noParamOrder);
        }
    }
}
