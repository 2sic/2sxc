using System.Collections.Generic;
using ToSic.Lib.Coding;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data;

[PublicApi]
public interface ITypedStack: ITyped, ICanDebug
{
    /// <inheritdoc cref="ITypedRelationships.Child"/>
    ITypedItem Child(string name, NoParamOrder noParamOrder = default, bool? required = default);

    /// <inheritdoc cref="ITypedRelationships.Children"/>
    IEnumerable<ITypedItem> Children(string field = default, NoParamOrder noParamOrder = default, string type = default, bool? required = default);
}