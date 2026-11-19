using ToSic.Sxc.Data;

namespace Custom.Data;

/// <summary>
/// Marker interface to mark all custom data models inheriting from <see cref="CustomItem"/> and <see cref="CustomModel"/>,
/// to sometimes differentiate if say an <see cref="ITypedItem"/> is generic or a custom one.
/// </summary>
/// <remarks>
/// Added v23.
/// </remarks>
public interface ICustomModel;
