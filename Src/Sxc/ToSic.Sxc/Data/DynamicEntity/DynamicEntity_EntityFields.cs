using System;
using ToSic.Eav.Data;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        public int EntityId => Entity?.EntityId ?? 0;


        /// <inheritdoc />
        public Guid EntityGuid => Entity?.EntityGuid ?? Guid.Empty;


        /// <inheritdoc />
        public IField Field(string name) => (this as ITypedItem).Field(name, required: null);

        IField ITypedItem.Field(string name, string noParamOrder, bool? required)
        {
            // TODO: make sure that if we use a path, the field is from the correct parent
            if (name.Contains(PropertyStack.PathSeparator.ToString()))
                throw new NotImplementedException("Path support on this method is not yet supported. Ask iJungleboy");

            return IsErrStrict(this, name, required, StrictGet)
                ? throw ErrStrict(name)
                : new Field(this, name, _Cdf);
        }

        /// <inheritdoc />
        public string EntityType => Entity?.Type?.Name;

    }
}
