using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Data.Model;

/// <summary>
/// Interface to mark objects which can receive <see cref="ITypedItem"/> objects and wrap them.
/// </summary>
/// <remarks>
/// Typical use is for custom data such as classes inheriting from <see cref="Custom.Data.CustomItem"/>
/// which takes an entity and then provides a strongly typed wrapper around it.
/// 
/// History
/// 
/// * Introduced in v17.02 under a slightly different name
/// * Made visible in the docs for better understanding in v19.01
/// * The `Setup()` method is still internal, as the signature may still change
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
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal void Setup(TContents baseItem, ICustomModelFactory modelFactory);
}