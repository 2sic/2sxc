
namespace ToSic.Sxc.Data.Models;

/// <summary>
/// BETA / WIP: Mark custom models/interfaces to specify what data they expect.
/// </summary>
/// <remarks>
/// This marks custom models to enable checks and more automation, such as:
///
/// * Specify an alternate content type name than the default, which would have to match the class/interface name
/// * Ensure that the model is only used for specific content-type(s) which don't match the model name
/// * Allow the model to be used with all content types `*`
/// * Automatically find the best stream of data to use with the model, if it doesn't match the model name
/// 
/// Typical use is for custom data such as classes inheriting from <see cref="Custom.Data.CustomItem"/>
/// which takes an entity and then provides a strongly typed wrapper around it.
/// 
/// History: New / WIP in v19.01
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("may change or rename at any time")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public sealed class ModelSourceAttribute: Attribute
{
    /// <summary>
    /// Determines which content-type names are expected when converting to this data model.
    /// </summary>
    /// <remarks>
    /// Usually this is checked when converting Entities to the custom data model.
    /// If it doesn't match, will then throw an error.
    /// 
    /// Typically just one value, such as "Article" or "Product".
    /// But it will also support "*" for anything, or (future!) a comma-separated list of content-type names.
    /// 
    /// History: WIP 19.01
    /// </remarks>
    public required string ContentTypes { get; init; }

    /// <summary>
    /// WIP, not officially supported yet.
    /// </summary>
    public string? Streams { get; init; }

    // TODO: MAKE INTERNAL AGAIN AFTER MOVING TO ToSic.Sxc.Custom
    [PrivateApi]
    public const string ForAnyContentType = "*";
}