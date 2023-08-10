using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data.Typed;

namespace ToSic.Sxc.Data
{
    public partial class DynamicStack: ITypedStack
    {
        [PrivateApi]
        bool ITyped.ContainsKey(string name)
        {
            return UnwrappedStack.Sources.Any(s =>
            {
                switch (s.Value)
                {
                    case null:
                        return false;
                    case ITyped typed:
                        return typed.ContainsKey(name);
                    case IHasKeys keyed:
                        return keyed.ContainsKey(name);
                }

                return false;
            });
            throw new NotImplementedException($"Not yet implemented on {nameof(ITypedStack)}");
        }

        // TODO: Keys()

        ITypedItem ITypedStack.Child(string name, string noParamOrder, bool? required)
        {
            var findResult = Helper.TryGet(name);
            return TypedHelpers.IsErrStrict(findResult.Found, required, Helper.StrictGet)
                ? throw TypedHelpers.ErrStrict(name)
                : _Cdf.AsItem(findResult.Result, noParamOrder);
        }

        IEnumerable<ITypedItem> ITypedStack.Children(string field, string noParamOrder, string type, bool? required)
        {
            // TODO: @2DM - type-filter of children is not applied
            var findResult = Helper.TryGet(field);
            return TypedHelpers.IsErrStrict(findResult.Found, required, Helper.StrictGet)
                ? throw TypedHelpers.ErrStrict(field)
                : _Cdf.AsItems(findResult.Result, noParamOrder);
        }
    }
}
