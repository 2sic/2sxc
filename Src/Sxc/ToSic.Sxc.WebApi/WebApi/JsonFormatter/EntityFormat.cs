using ToSic.Eav.DataFormats.EavLight;

namespace ToSic.Sxc.WebApi;

/// <summary>
/// Formats to use for automatic Entity to JSON conversion.
/// This is for the <see cref="JsonFormatterAttribute"/>.
/// As of now it only has `None` and `Light`, in future we plan to extend this with other formats.
/// Default is usually `Light`.
/// </summary>
[PublicApi]
public enum EntityFormat
{
    /// <summary>
    /// Do not auto-convert into any specific format.
    /// If <see cref="IEntity"/> objects are in the result, will result in an error.
    /// </summary>
    None,

    /// <summary>
    /// Format <see cref="IEntity"/> objects as <see cref="EavLightEntity"/>.
    /// This results in single-language objects with name/value pairs like a JavaScript object.
    /// </summary>
    Light,
}