using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent;
using ToSic.Sxc.Data;
using ToSic.Sxc.Edit.InPageEditingSystem;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Blocks.Renderers
{
    internal class Simple
    {
        private static string EmptyMessage = "<!-- auto-render of item {0} -->";

        internal static IHtmlString Render(IBlock parentCb, IEntity entity, ILog parentLog)
        {
            var log = new Log("Htm.Render", parentLog, "simple");

            // if not the expected content-type, just output a hidden html placeholder
            if (entity.Type.Name != Settings.AttributeSetStaticNameContentBlockTypeName)
            {
                log.Add("empty, will return hidden html placeholder");
                return new HtmlString(string.Format(EmptyMessage, entity.EntityId));
            }

            // render it
            log.Add("found, will render");
            var cb = new BlockFromEntity(parentCb, entity, log);
            return cb.CmsInstance.Render();
        }

        private const string WrapperTemplate = "<div class='{0}' {1}>{2}</div>";
        private const string WrapperMultiItems = "sc-content-block-list"; // tells quickE that it's an editable area
        private const string WrapperSingleItem = WrapperMultiItems + " show-placeholder single-item"; // enables a placeholder when empty, and limits one entry


        internal static string RenderWithEditContext(DynamicEntity parent, IDynamicEntity subItem, string cbFieldName,  Guid? newGuid = null, IInPageEditingSystem edit = null)
        {
            if (edit == null)
                edit = new InPageEditingHelper(parent.CmsInstance, parent.CmsInstance.Log);

            var attribs = edit.ContextAttributes(parent, field: cbFieldName, newGuid: newGuid);
            var inner = subItem == null ? "": Render(parent.CmsInstance.Block, subItem.Entity, parent.CmsInstance.Log).ToString();
            var cbClasses = edit.Enabled ? WrapperSingleItem : "";
            return string.Format(WrapperTemplate, new object[] { cbClasses, attribs, inner});
        }

        internal static string RenderListWithContext(DynamicEntity parent, string fieldName, IInPageEditingSystem edit = null)
        {
            var innerBuilder = new StringBuilder();
            var found = parent.TryGetMember(fieldName, out var objFound);
            if (found)
            {
                if (objFound is IList<DynamicEntity> itms)
                    foreach (var cb in itms)
                        innerBuilder.Append(Render(cb.CmsInstance.Block, cb.Entity, parent.CmsInstance.Log));
            }

            // create edit object if missing...to re-use in the wh
            if (edit == null)
                edit = new InPageEditingHelper(parent.CmsInstance, parent.CmsInstance.Log);

            return string.Format(WrapperTemplate, new object[]
            {
                edit.Enabled ? WrapperMultiItems : "",
                edit.ContextAttributes(parent, field: fieldName),
                innerBuilder
            });
        }
    }
}