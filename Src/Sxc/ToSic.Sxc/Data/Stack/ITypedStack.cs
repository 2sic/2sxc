using ToSic.Sxc.Data.Internal.Docs;

namespace ToSic.Sxc.Data;

[PublicApi]
public interface ITypedStack: ITyped, ICanDebug
{
    /// <inheritdoc cref="ITypITypedRelationshipsDocsld"/>
    ITypedItem Child(string name, NoParamOrder noParamOrder = default, bool? required = default);

    /// <inheritdoc cref="ITypedRelationshipsDocs.Children"/>
    IEnumerable<ITypedItem> Children(string field = default, NoParamOrder noParamOrder = default, string type = default, bool? required = default);
}