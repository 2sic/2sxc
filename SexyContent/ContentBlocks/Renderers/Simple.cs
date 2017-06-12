using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using ToSic.SexyContent.Edit.InPageEditingSystem;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.ContentBlocks.Renderers
{
    internal class Simple
    {
        private static string EmptyMessage = "<!-- auto-render of item {0} -->";

        internal static IHtmlString Render(IContentBlock parentCb, ToSic.Eav.Interfaces.IEntity entity)
        {
            
            // if not the expected content-type, just output a hidden html placeholder
            if (entity.Type.Name != Settings.AttributeSetStaticNameContentBlockTypeName)
                return new HtmlString(string.Format(EmptyMessage, entity.EntityId));

            // render it
            var cb = new EntityContentBlock(parentCb, entity);
            return cb.SxcInstance.Render();
        }

        private const string WrapperTemplate = "<div class='{0}' {1}>{2}</div>";
        private const string WrapperMultiItems = "sc-content-block-list"; // tells quickE that it's an editable area
        private const string WrapperSingleItem = WrapperMultiItems + " show-placeholder single-item"; // enables a placeholder when empty, and limits one entry

        internal static string RenderWithEditContext(DynamicEntity parent, DynamicEntity subItem, string cbFieldName,  Guid? newGuid = null, IInPageEditingSystem edit = null)
        {
            if (edit == null)
                edit = new InPageEditingHelper(parent.SxcInstance);

            var attribs = edit.ContextAttributes(parent, field: cbFieldName, newGuid: newGuid);
            var inner = (subItem == null) ? "": Render(parent.SxcInstance.ContentBlock, subItem.Entity).ToString();
            var cbClasses = edit.Enabled ? WrapperSingleItem : "";
            return string.Format(WrapperTemplate, new object[] { cbClasses, attribs, inner});
        }



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
                        innerBuilder.Append(Render(cb.SxcInstance.ContentBlock, cb.Entity));
            }

            // create edit object if missing...to re-use in the wh
            if (edit == null)
                edit = new InPageEditingHelper(parent.SxcInstance);

            return string.Format(WrapperTemplate, new object[]
            {
                edit.Enabled ? WrapperMultiItems : "",
                edit.ContextAttributes(parent, field: fieldName),
                innerBuilder
            });

        }

    }
}