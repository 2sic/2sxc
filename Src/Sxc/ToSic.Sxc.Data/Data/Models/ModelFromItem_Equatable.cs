
namespace ToSic.Sxc.Data.Models;

partial class ModelFromItem : IMultiWrapper<IEntity>, IEquatable<ITypedItem>
{
    bool IEquatable<ITypedItem>.Equals(ITypedItem other) => Equals(other);

    /// <summary>
    /// Ensure that the equality check is done correctly.
    /// If two objects wrap the same item, they will be considered equal.
    /// </summary>
    public override bool Equals(object b)
        => MultiWrapperEquality.EqualsObj(this, b);

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public override int GetHashCode()
        => MultiWrapperEquality.GetWrappedHashCode(this);

    IEntity IMultiWrapper<IEntity>.RootContentsForEqualityCheck
        => (_item as IMultiWrapper<IEntity>)?.RootContentsForEqualityCheck;

    /// <summary>
    /// Ensure that the equality check is done correctly.
    /// If two objects wrap the same item, they will be considered equal.
    /// </summary>
    /// <param name="a">first item to compare</param>
    /// <param name="b">second item to compare</param>
    /// <returns>true, if both wrappers are the same type and wrap the same entity</returns>
    public static bool operator ==(ModelFromItem a, ModelFromItem b)
        => MultiWrapperEquality.IsEqual(a, b);

    /// <summary>
    /// Ensure that the equality check is done correctly.
    /// If two objects wrap the same item, they will be considered equal.
    /// </summary>
    /// <param name="a">first item to compare</param>
    /// <param name="b">second item to compare</param>
    /// <returns>false, if both wrappers are the same type and wrap the same entity</returns>
    public static bool operator !=(ModelFromItem a, ModelFromItem b)
        => !MultiWrapperEquality.IsEqual(a, b);
}