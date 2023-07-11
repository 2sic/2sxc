using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public interface ITypedStack: ITyped, ICanDebug
    {
        /// <inheritdoc cref="ITypedRelationships.Child"/>
        ITypedItem Child(string name, string noParamOrder = Protector);

        /// <inheritdoc cref="ITypedRelationships.Children"/>
        IEnumerable<ITypedItem> Children(string field = default, string noParamOrder = Protector, string type = default);
    }
}
