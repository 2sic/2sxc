namespace ToSic.Sxc.Data.Model;

/// <summary>
/// BETA / WIP: Mark DataModel objects/interfaces and specify what ContentType they are for.
/// </summary>
/// <remarks>
/// Typical use is for custom data such as classes inheriting from <see cref="Custom.Data.CustomItem"/>
/// which takes an entity and then provides a strongly typed wrapper around it.
/// 
/// History: New / WIP in v19.01
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("may change or rename at any time")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DataModelAttribute: Attribute
{
    /// <summary>
    /// Determines which content-type names are expected when converting to this data model.
    /// </summary>
    /// <remarks>
    /// Usually this is checked when converting Entities to the custom data model.
    /// If it doesn't match, will then throw an error.
    /// 
    /// Typically just one value, such as "Article" or "Product".
    /// But it will also support "*" for anything, or a comma-separated list of content-type names.
    /// 
    /// History: WIP 19.01
    /// </remarks>
    public string ForContentTypes { get; init; }

    public string StreamNames { get; init; }

    /// <summary>
    /// Just custom remarks, no technical functionality.
    /// </summary>
    public string Remarks { get; init; }

    public const string ForAnyContentType = "*";
}