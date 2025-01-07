using ToSic.Sxc.Data;

namespace ToSic.Sxc.Models;

/// <summary>
/// Interface to mark objects which can receive data such as <see cref="IEntity"/> or <see cref="ITypedItem"/> objects and wrap them.
/// Typical use is for custom data such as classes inheriting from <see cref="Custom.Data.CustomItem"/>
/// which takes an entity and then provides a strongly typed wrapper around it.
/// </summary>
/// <remarks>
/// * Introduced in v17.02 under a slightly different name
/// * Made visible in the docs for better understanding in v19.01
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("may change or rename at any time")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IDataModelForType: IDataModel
{
    /// <summary>
    /// Internal functionality, so the object can declare what content Type it's for.
    ///
    /// By default, it will use the content-type name, but providing this property would allow
    /// other classes (with different names) to provide the proper name.
    ///
    /// ATM it's only used in App.Data.GetOne{T} and App.Data.GetAll{T}
    /// </summary>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    string ForContentType { get; }

}