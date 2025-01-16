namespace ToSic.Sxc.Data.Model;

/// <summary>
/// Interface to mark objects which can receive data such as <see cref="IEntity"/> or <see cref="ITypedItem"/> objects and wrap them,
/// usually into a strongly typed model.
/// </summary>
/// <remarks>
/// Typical use is for custom data such as classes inheriting from <see cref="Custom.Data.CustomItem"/>
/// which takes an entity and then provides a strongly typed wrapper around it.
/// 
/// History
/// 
/// * Made visible in the docs for better understanding in v19.01
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("may change or rename at any time")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IDataModel;