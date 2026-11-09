
namespace ToSic.Sxc.Data;

/// <summary>
/// The stack as typed object.
/// </summary>
/// <remarks>
/// Enhanced in v17.07 to fully support `ITypedItem`, before it only supported `ITyped`.
/// </remarks>
[PublicApi]
public interface ITypedStack : ITypedItem, ICanDebug
{
    [PrivateApi("Internal use only, not meant for public consumption")]
    ITypedStack GetStack(string[] names);
}