namespace ToSic.Sxc.Data.Internal.Convert;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IHasJsonSource
{
    object JsonSource { get; }
}