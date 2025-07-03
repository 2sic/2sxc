using ToSic.Razor.Blade;

namespace ToSic.Sxc.Render.Sys.ModuleHtml;

[PrivateApi("Probably always internal, as there is probably no reason to make it public")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IModuleHtmlService: IHasLog
{
    /// <summary>
    /// Tags added by code, errors, TurnOn etc. which are added to the end of the module.
    /// Retrieves and clears stored tags for the specified module to prevent duplicate data accumulation.
    /// </summary>
    /// <param name="moduleId">The ID of the module.</param>
    /// <returns>A read-only collection of HTML tags associated with the module.</returns>
    /// <remarks>
    /// The .net Framework implementation (DNN) will ignore the ModuleId.
    /// </remarks>
    IReadOnlyCollection<IHtmlTag> GetMoreTagsAndFlush(int moduleId);

    /// <summary>
    /// Add a tag (like a TurnOn) to the end of the module
    /// Adds an HTML tag to the collection of tags to be rendered at the end of the module,
    /// optionally preventing duplicates and scoping to a specific module ID.
    /// </summary>
    /// <param name="tag">The HTML tag to add.</param>
    /// <param name="moduleId">
    ///     The ID of the module to which the tag should be scoped; defaults to the current module.
    /// </param>
    /// <param name="nameId">
    ///     An optional identifier for the tag; if not provided, the string representation of the tag is used.
    ///     This identifier helps prevent duplicate tags if <paramref name="noDuplicates"/> is set to true.
    /// </param>
    /// <param name="noDuplicates">
    ///     If true, the tag will only be added if it does not already exist in the collection.
    /// </param>
    /// <remarks>
    /// The .net Framework implementation (DNN) will ignore the ModuleId.
    /// </remarks>
    void AddTag(IHtmlTag tag, int moduleId, string? nameId = null, bool noDuplicates = false);
}