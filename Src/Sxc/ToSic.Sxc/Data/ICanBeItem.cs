using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Data;

/// <summary>
/// This is just a helper interface.
/// Reason is that we have different types of objects representing real entity-data such as:
/// 
/// * <see cref="IDynamicEntity"/>
/// * <see cref="ITypedItem"/>
///
/// To make sure that APIs which use this have a consistent structure, these objects all implement this interface.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("Just FYI")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ICanBeItem: ICanBeEntity
{
    [PrivateApi]
    IBlock TryGetBlockContext();

    [PrivateApi]
    ITypedItem Item { get; }
}