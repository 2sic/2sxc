using System;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
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
        public IField Field(string name) => ItemHelper.Field(this, name);

        [PrivateApi]
        IField ITypedItem.Field(string name, string noParamOrder, bool? required) => ItemHelper.Field(this, name, noParamOrder, required);


        /// <inheritdoc />
        public string EntityType => Entity?.Type?.Name;

    }
}
