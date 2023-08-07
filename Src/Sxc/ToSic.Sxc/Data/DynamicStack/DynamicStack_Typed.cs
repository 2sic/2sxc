using System;
using System.Collections.Generic;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    public partial class DynamicStack: ITypedStack
    {
        [PrivateApi]
        bool ITyped.ContainsKey(string name)
        {
            throw new NotImplementedException($"Not yet implemented on {nameof(ITypedStack)}");
        }

        ///// <summary>
        ///// This error is used a lot, when accessing primary properties of ITypedItem, since it's simply not supported.
        ///// </summary>
        //private const string ErrNotSupported =
        //    "You are trying to access '{0}'. This is a merged object containing multiple other sources. " +
        //    "So there is no real/primary '{0}' property it can provide. " +
        //    "If you need it, get it from the part you need which was used to create this merged object. " +
        //    "If you see this error when using a more advanced API such as the ImageService, make sure you give it the correct original object, not the merged object. ";

        //private string CreateErrMsg(string addOn = default, [CallerMemberName] string cName = default)
        //{
        //    return string.Format(ErrNotSupported, cName);
        //}

        ITypedItem ITypedStack.Child(string name, string noParamOrder, bool? required)
        {
            var findResult = GetInternal(name, lookup: false);
            return IsErrStrict(findResult.Found, required, StrictGet)
                ? throw ErrStrict(name)
                : _Services.AsC.AsItem(findResult.Result, noParamOrder);
        }

        IEnumerable<ITypedItem> ITypedStack.Children(string field, string noParamOrder, string type, bool? required)
        {
            // TODO: @2DM - type-filter of children is not applied
            var findResult = GetInternal(field, lookup: false);
            return IsErrStrict(findResult.Found, required, StrictGet)
                ? throw ErrStrict(field)
                : _Services.AsC.AsItems(findResult.Result, noParamOrder);
        }
    }
}
