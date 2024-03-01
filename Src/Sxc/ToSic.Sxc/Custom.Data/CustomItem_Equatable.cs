using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace Custom.Data;

partial class CustomItem: IMultiWrapper<IEntity>
{
    bool IEquatable<ITypedItem>.Equals(ITypedItem other) => Equals(other);

    public override bool Equals(object b) => MultiWrapperEquality.EqualsObj(this, b);

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override int GetHashCode() => MultiWrapperEquality.GetWrappedHashCode(this);

    IEntity IMultiWrapper<IEntity>.RootContentsForEqualityCheck => (_item as IMultiWrapper<IEntity>)?.RootContentsForEqualityCheck;
}