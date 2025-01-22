using ToSic.Sxc.Data.Models;

// ReSharper disable once CheckNamespace
namespace Custom.Data;

/// <summary>
/// Base class for custom models. Similar to <see cref="CustomItem"/> but without predefined public properties or methods.
/// </summary>
/// <remarks>
/// This is a lightweight custom object which doesn't have public properties
/// like `Id` or methods such as `String(...)`.
///
/// It's ideal for data models which need full control,
/// like for serializing or just to reduce the API surface.
///
/// You can access the underlying (protected) `_item` property to get the raw data.
/// And it also has the (protected) `As&lt;...&gt;()` conversion for typed sub-properties.
///
/// History: New in 19.03
/// </remarks>
[PublicApi]
[ModelSource(ContentTypes = "*")]
public class CustomModel: ModelFromItem;