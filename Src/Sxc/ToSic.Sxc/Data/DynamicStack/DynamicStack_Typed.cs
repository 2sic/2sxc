using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Documentation;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

namespace ToSic.Sxc.Data
{
    public partial class DynamicStack: ITypedStack
    {
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
    }
}
