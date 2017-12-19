using System;
using System.Web;
using ToSic.SexyContent.ContentBlocks.Renderers;

namespace ToSic.SexyContent.ContentBlocks
{
    public class Render
    {
        /// <summary>
        /// Render one content block
        /// This is accessed through DynamicEntity.Render()
        /// At the moment it MUST stay internal, as it's not clear what API we want to surface
        /// </summary>
        /// <param name="context">The parent-item containing the content-blocks and providing edit-context</param>
        /// <param name="dontRelyOnParameterOrder"></param>
        /// <param name="item">The content-block item to render</param>
        /// <param name="field">Optional: </param>
        /// <param name="newGuid"></param>
        /// <returns></returns>
        public static IHtmlString One(DynamicEntity context,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter,
            DynamicEntity item = null, 
            string field = null,
            Guid? newGuid = null)
        {
            Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "One");
            if (item == null)
                item = context;
            
            return field == null
                ? Simple.Render(context.SxcInstance.ContentBlock, item.Entity, context.SxcInstance.Log) // with edit-context
                : new HtmlString(Simple.RenderWithEditContext(context, item, field, newGuid) + "<b>data-list-context</b>"); // data-list-context (no edit-context)
        }

        /// <summary>
        /// Render content-blocks into a larger html-block containing placeholders
        /// </summary>
        /// <param name="context">The parent-item containing the content-blocks and providing edit-context</param>
        /// <param name="dontRelyOnParameterOrder"></param>
        /// <param name="field">Field containing the content-blocks</param>
        /// <param name="merge">Optional: html-text containing special placeholders</param>
        /// <returns></returns>
        public static IHtmlString All(DynamicEntity context,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter,
            string field = null, 
            string merge = null)
        {
            Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "All");
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            return (merge == null)
                ? new HtmlString(Simple.RenderListWithContext(context, field))
                : new HtmlString(InTextContentBlocks.Render(context, field, merge));
        }
    }
}