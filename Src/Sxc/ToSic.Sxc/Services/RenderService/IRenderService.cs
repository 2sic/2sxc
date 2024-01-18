using ToSic.Razor.Markup;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Services;

/// <summary>
/// Block-Rendering system. It's responsible for taking a Block and delivering HTML for the output. <br/>
/// It's used for InnerContent, so that Razor-Code can easily render additional content blocks. <br/>
/// See also [](xref:Basics.Cms.InnerContent.Index)
/// </summary>
/// <remarks>
/// This replaces the now obsolete ToSic.Sxc.Blocks.Render
///
/// History
/// * Introduced in v12.05 but on another namespace which still works for compatibility
/// * Moved to ToSic.Sxc.Services in v13
/// </remarks>
[PublicApi]
public interface IRenderService
{
    /// <summary>
    /// Render one content block
    /// This is accessed through DynamicEntity.Render()
    /// At the moment it MUST stay internal, as it's not clear what API we want to surface
    /// </summary>
    /// <param name="parent">The parent-item containing the content-blocks and providing edit-context</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="item">The content-block item to render. Optional, by default the same item is used as the context.</param>
    /// <param name="field">Optional: </param>
    /// <param name="newGuid">Internal: this is the guid given to the item when being created in this block. Important for the inner-content functionality to work. </param>
    /// <param name="data">Data to give the Razor as `DynamicModel` - new 15.07</param>
    /// <returns></returns>
    /// <remarks>
    /// * Changed result object to `IRawHtmlString` in v16.02 from `IHybridHtmlString`
    /// </remarks>
    IRawHtmlString One(
        ICanBeItem parent,
        NoParamOrder noParamOrder = default,
        ICanBeEntity item = null,
        object data = null,
        string field = null,
        Guid? newGuid = null
    );

    /// <summary>
    /// Render content-blocks into a larger html-block containing placeholders
    /// </summary>
    /// <param name="parent">The parent-item containing the content-blocks and providing edit-context</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="field">Required: Field containing the content-blocks. </param>
    /// <param name="max">BETA / WIP</param>
    /// <param name="merge">Optional: html-text containing special placeholders.</param>
    /// <param name="apps">BETA / WIP</param>
    /// <returns></returns>
    /// <remarks>
    /// * Changed result object to `IRawHtmlString` in v16.02 from `IHybridHtmlString`
    /// </remarks>
    IRawHtmlString All(
        ICanBeItem parent,
        NoParamOrder noParamOrder = default,
        string field = null,
        string apps = null,
        int max = 100,
        string merge = null
    );

    /// <summary>
    /// Get a 2sxc module rendered directly. 
    /// </summary>
    /// <param name="pageId"></param>
    /// <param name="moduleId"></param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="data">Data to give the Razor as `DynamicModel` - new 15.07</param>
    /// <returns>
    /// An HTML-String which can be added to the output directly.
    /// The object also has additional information like assets or page changes, which are not applied when using this render command. 
    /// </returns>
    /// <remarks>New in 2sxc 13.02</remarks>
    IRenderResult Module(
        int pageId,
        int moduleId,
        NoParamOrder noParamOrder = default,
        object data = null
    );
}