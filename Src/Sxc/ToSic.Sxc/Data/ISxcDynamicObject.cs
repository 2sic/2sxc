namespace ToSic.Sxc.Data;

/// <summary>
/// Marks 2sxc dynamic objects.
/// Mainly to ensure that they are not re-converted if they already are such dynamic objects
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ISxcDynamicObject //: ITyped
{
}