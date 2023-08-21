using System;
using ToSic.Eav.Data;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

namespace ToSic.Sxc.Data
{
    public partial class CodeDataFactory
    {
        public IField Field(ITypedItem parent, string name, bool propsRequired, string noParamOrder = Protector, bool? required = default)
        {
            Protect(noParamOrder, nameof(required));
            // TODO: make sure that if we use a path, the field is from the correct parent
            if (name.Contains(PropertyStack.PathSeparator.ToString()))
                throw new NotImplementedException("Path support on this method is not yet supported. Ask iJungleboy");

            return IsErrStrict(parent, name, required, propsRequired)
                ? throw ErrStrictForTyped(parent, name)
                : new Field(parent, name, this);
        }

    }
}
