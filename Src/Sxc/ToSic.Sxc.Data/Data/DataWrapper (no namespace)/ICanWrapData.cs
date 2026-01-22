namespace ToSic.Sxc.Data;

/// <summary>
/// Marks objects such as custom items or data models, which can wrap data (usually Entities or TypedItems).
/// </summary>
/// <remarks>
/// Most 2sxc base classes such as Razor files or WebApi files have some methods which will return a typed data model.
/// This is usually `As&lt;TModel&gt;()` or `AsList&lt;TModel&gt;()`.
/// To help the developer understand what is allowed for `TModel` we must mark all objects which are supported, and this is done with this interface.
///
/// So any class which is meant to wrap data from an <see cref="IEntity"/> or <see cref="ITypedItem"/> should implement this interface.
/// Typical use is for custom data such as classes inheriting from [](xref:Custom.Data.CustomItem)
/// which takes an entity and then provides a strongly typed wrapper around it.
/// 
/// History
/// 
/// * Made visible in the docs for better understanding in v19.01
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("may change or rename at any time")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICanWrapData;