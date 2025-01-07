namespace ToSic.Sxc.Data;

/// <summary>
/// Interface to mark objects which can receive <see cref="ITypedItem"/> objects and wrap them.
/// Typical use is for custom data such as classes inheriting from <see cref="Custom.Data.CustomItem"/>
/// which takes an entity and then provides a strongly typed wrapper around it.
/// </summary>
/// <remarks>
/// * Introduced in v17.02 under a slightly different name
/// * Made visible in the docs for better understanding in v19.01
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("may change or rename at any time")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IDataModelOf<in TContents>: IDataModel
{
    /// <summary>
    /// Add the data to use for the wrapper.
    /// We are not doing this in the constructor,
    /// because the object needs to have an empty (or future: maybe DI-compatible) constructor. 
    /// </summary>
    /// <param name="baseItem"></param>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal void Setup(TContents baseItem);
}