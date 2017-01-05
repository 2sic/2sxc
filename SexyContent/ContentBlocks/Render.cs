using System;
using System.Web;
using ToSic.Eav;
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
        /// <param name="parent"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IHtmlString One(DynamicEntity parent, DynamicEntity item)
            => Simple.Render(parent.SxcInstance.ContentBlock, item.Entity);

        public static IHtmlString OneWithContext(DynamicEntity parent, DynamicEntity item, 
            string contextFieldName, Guid? newGuid = null)
            => new HtmlString(Simple.RenderWithEditContext(parent, item, contextFieldName, newGuid));

        public static IHtmlString AllWithContext(DynamicEntity parent, string entitiesFieldName)
            => new HtmlString(Simple.RenderListWithContext(parent, entitiesFieldName));
        

        /// <summary>
        /// Render content-blocks into a larger html-block containing placeholders
        /// </summary>
        /// <param name="parent">The parent-item containing the content-blocks and providing edit-context</param>
        /// <param name="entitiesFieldName">Field containing the content-blocks</param>
        /// <param name="textWithPlaceholders">The html-text containing the placeholders</param>
        /// <returns></returns>
        public static IHtmlString IntoPlaceholders(DynamicEntity parent,
            string entitiesFieldName, string textWithPlaceholders)
            => new HtmlString(new InTextContentBlocks()
                .Render(parent, entitiesFieldName, textWithPlaceholders));

    }
}