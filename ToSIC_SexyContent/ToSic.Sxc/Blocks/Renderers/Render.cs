using System;
using System.Web;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks.Renderers;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// Block-Rendering system. It's responsible for taking a Block and delivering HTML for the output. <br/>
    /// It's used for InnerContent, so that Razor-Code can easily render additional content blocks. <br/>
    /// See also [](xref:Specs.Cms.InnerContent)
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public class Render
    {
        /// <summary>
        /// Render one content block
        /// This is accessed through DynamicEntity.Render()
        /// At the moment it MUST stay internal, as it's not clear what API we want to surface
        /// </summary>
        /// <param name="context">The parent-item containing the content-blocks and providing edit-context</param>
        /// <param name="dontRelyOnParameterOrder"></param>
        /// <param name="item">The content-block item to render. Optional, by default the same item is used as the context.</param>
        /// <param name="field">Optional: </param>
        /// <param name="newGuid">Internal: this is the guid given to the item when being created in this block. Important for the inner-content functionality to work. </param>
        /// <returns></returns>
        public static IHtmlString One(DynamicEntity context,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            IDynamicEntity item = null, 
            string field = null,
            Guid? newGuid = null)
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, nameof(One), $"{nameof(item)},{nameof(field)},{nameof(newGuid)}");
            if (item == null)
                item = context;
            
            return field == null
                ? Simple.Render(context.CmsBlock.Block, item.Entity, context.CmsBlock.Log) // with edit-context
                : new HtmlString(Simple.RenderWithEditContext(context, item, field, newGuid) + "<b>data-list-context</b>"); // data-list-context (no edit-context)
        }

        /// <summary>
        /// Render content-blocks into a larger html-block containing placeholders
        /// </summary>
        /// <param name="context">The parent-item containing the content-blocks and providing edit-context</param>
        /// <param name="dontRelyOnParameterOrder"></param>
        /// <param name="field">Required: Field containing the content-blocks. </param>
        /// <param name="merge">Optional: html-text containing special placeholders.</param>
        /// <returns></returns>
        public static IHtmlString All(DynamicEntity context,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string field = null, 
            string merge = null)
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, nameof(All), $"{nameof(field)},{nameof(merge)}");
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            return merge == null
                ? new HtmlString(Simple.RenderListWithContext(context, field))
                : new HtmlString(InTextContentBlocks.Render(context, field, merge));
        }
    }
}