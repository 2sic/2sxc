using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using ToSic.Eav;
using ToSic.SexyContent.Edit.InPageEditingSystem;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.ContentBlocks.Renderers
{
    internal class Simple
    {
        private static string EmptyMessage = "<!-- auto-render of item {0} -->";

        internal static IHtmlString Render(IContentBlock parentCb, IEntity entity)
        {
            
            // if not the expected content-type, just output a hidden html placeholder
            if (entity.Type.Name != Settings.AttributeSetStaticNameContentBlockTypeName)
                return new HtmlString(string.Format(EmptyMessage, entity.EntityId));

            // render it
            var cb = new EntityContentBlock(parentCb, entity);
            return cb.SxcInstance.Render();
        }

        private const string Div = "<div class='sc-content-block-list show-placeholder single-item' {0}>{1}</div>";

        internal static string RenderWithEditContext(DynamicEntity parent, DynamicEntity subItem, string cbFieldName,  Guid? newGuid = null, IInPageEditingSystem edit = null)
        {
            if (edit == null)
                edit = new InPageEditingHelper(parent.SxcInstance);

            return string.Format(Div,
                edit.ContextAttributes(parent, field: cbFieldName, newGuid: newGuid),
                Render(subItem.SxcInstance.ContentBlock, subItem.Entity));
        }

        private const string ListWrapperTemplate = "<div class='sc-content-block-list' {0}>{1}</div>";

        internal static string RenderListWithContext(DynamicEntity parent, string fieldName, IInPageEditingSystem edit = null)
        {
            object objFound;
            var innerBuilder = new StringBuilder();
            var found = parent.TryGetMember(fieldName, out objFound);
            if (found)
            {
                var itms = objFound as IList<DynamicEntity>;
                if(itms != null)
                    foreach (var cb in itms)
                        innerBuilder.Append(cb.Render());
            }

            // create edit object if missing...
            if (edit == null)
                edit = new InPageEditingHelper(parent.SxcInstance);

            return string.Format(ListWrapperTemplate, 
                edit.ContextAttributes(parent, field: fieldName), 
                innerBuilder);

        }

    }
}