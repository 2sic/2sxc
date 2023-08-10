using System;
using System.Runtime.CompilerServices;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

namespace ToSic.Sxc.Data
{
    internal class CodeItemHelper
    {
        public readonly IPropertyLookup Parent;
        public readonly CodeEntityHelper Helper;

        public CodeItemHelper(IPropertyLookup parent, CodeEntityHelper helper)
        {
            Parent = parent;
            Helper = helper;
        }

        #region Get

        public object Get(string name, string noParamOrder, bool? required, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(required), methodName: cName);
            var findResult = Helper.TryGet(name);
            return IsErrStrict(findResult.Found, required, Helper.StrictGet)
                ? throw ErrStrict(name, cName)
                : findResult.Result;
        }

        public TValue G4T<TValue>(string name, string noParamOrder, TValue fallback, bool? required = default, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(fallback), methodName: cName);
            var findResult = Helper.TryGet(name);
            return IsErrStrict(findResult.Found, required, Helper.StrictGet)
                ? throw ErrStrict(name, cName)
                : findResult.Result.ConvertOrFallback(fallback);
        }


        #endregion


        public IField Field(ITypedItem parent, string name, string noParamOrder = Protector, bool? required = default)
        {
            Protect(noParamOrder, nameof(required));
            // TODO: make sure that if we use a path, the field is from the correct parent
            if (name.Contains(PropertyStack.PathSeparator.ToString()))
                throw new NotImplementedException("Path support on this method is not yet supported. Ask iJungleboy");

            return IsErrStrict(parent, name, required, Helper.StrictGet)
                ? throw ErrStrict(name)
                : new Field(parent, name, Helper.Cdf);
        }

    }
}
