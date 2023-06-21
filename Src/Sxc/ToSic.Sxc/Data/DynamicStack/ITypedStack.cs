using System.Collections.Generic;
using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public interface ITypedStack: ITypedRead
    {
        /// <inheritdoc cref="ITypedRelationships.Child"/>
        ITypedItem Child(string name);

        /// <inheritdoc cref="ITypedRelationships.Children"/>
        IEnumerable<ITypedItem> Children(string field = default, string noParamOrder = Protector, string type = default);
    }
}
