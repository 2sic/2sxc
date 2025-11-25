
namespace ToSic.Sxc.Data;

/// <summary>
/// The stack as typed object.
/// </summary>
/// <remarks>
/// Enhanced in v17.07 to fully support `ITypedItem`, before it only supported `ITyped`.
/// </remarks>
[PublicApi]
public interface ITypedStack: ITypedItem, ICanDebug
{
    ///// <inheritdoc cref="ITypITypedRelationshipsDocsld"/>
    //ITypedItem Child(string name, NoParamOrder npo = default, bool? required = default);

    ///// <inheritdoc cref="ITypedRelationshipsDocs.Children"/>
    //IEnumerable<ITypedItem> Children(string field = default, NoParamOrder npo = default, string type = default, bool? required = default);
}