using ToSic.Razor.Blade;
using ToSic.Sxc.Services.OutputCache;

namespace ToSic.Sxc.Render.Output.Sys;

[PrivateApi("Probably always internal, as there is probably no reason to make it public")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IModulesOutputService: IHasLog
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
    /// 
    /// <param name="moduleId">
    /// The ID of the module to which the tag should be scoped; defaults to the current module.
    /// </param>
    /// <param name="tag">The HTML tag to add.</param>
    /// <param name="noDuplicates">
    /// If true, the tag will only be added if it does not already exist in the collection.
    /// </param>
    /// 
    /// <remarks>
    /// The .net Framework implementation (DNN) will ignore the ModuleId.
    /// </remarks>
    /// <returns>
    /// The same tag if it was added, or null if it was not added due to duplication.
    /// </returns>
    IHtmlTag? AddTag(int moduleId, IHtmlTag tag, bool noDuplicates = false);

    /// <summary>
    /// Configure the output cache for a specific module.
    /// </summary>
    /// <param name="moduleId">The ID of the module.</param>
    /// <param name="settings">The output cache settings to apply.</param>
    /// <remarks>
    /// Output-cache settings and DependOn(...) calls can happen in any order during one render,
    /// so we keep a small per-module buffer until the final render result is assembled.
    /// </remarks>
    void ConfigureOutputCache(int moduleId, OutputCacheSettings settings);

    /// <summary>
    /// Add a dependency key for the output cache of a specific module.
    /// </summary>
    /// <param name="moduleId">The ID of the module.</param>
    /// <param name="key">The dependency key to add.</param>
    void AddOutputCacheDependency(int moduleId, string key);

    /// <summary>
    /// Get the output cache settings for a specific module.
    /// </summary>
    /// <param name="moduleId">The ID of the module.</param>
    /// <returns>The output cache settings, or null if none are configured.</returns>
    OutputCacheSettings? GetOutputCache(int moduleId);

    /// <summary>
    /// Add a hint to show to superusers only; in future possibly also other users.
    /// </summary>
    /// <param name="moduleId">The ID of the module to add the hint for.</param>
    /// <param name="hint">The hint to add.</param>
    void AddHint(int moduleId, ModuleHint hint);

    /// <summary>
    /// Get all hints for a module and flush them, so they won't be returned again.
    /// </summary>
    /// <param name="moduleId">The ID of the module to get the hints for.</param>
    /// <returns>A read-only collection of module hints.</returns>
    IList<ModuleHint> GetHintsAndFlush(int moduleId = default);
}