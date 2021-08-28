using System;
using System.Collections.Generic;
using System.Text;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;
using ToSic.Sxc.Edit.InPageEditingSystem;
using ToSic.Sxc.Web;
#if NET451
using HtmlString = System.Web.HtmlString;
using IHtmlString = System.Web.IHtmlString;
#else
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif



namespace ToSic.Sxc.Blocks.Renderers
{
    internal class Simple
    {
        private static string EmptyMessage = "<!-- auto-render of item {0} -->";

        internal static string Render(IBlock parentBlock, IEntity entity)
        {
            var log = new Log("Htm.Render", parentBlock.Log, "simple");

            // if not the expected content-type, just output a hidden html placeholder
            if (entity.Type.Name != Settings.AttributeSetStaticNameContentBlockTypeName)
            {
                log.Add("empty, will return hidden html placeholder");
                return string.Format(EmptyMessage, entity.EntityId);
            }

            // render it
            log.Add("found, will render");
            var cb = parentBlock.Context.ServiceProvider.Build<BlockFromEntity>().Init(parentBlock, entity, log);
            return cb.BlockBuilder.Render();
        }

        private const string WrapperTemplate = "<div class='{0}' {1}>{2}</div>";
        private const string WrapperMultiItems = "sc-content-block-list"; // tells quickE that it's an editable area
        private const string WrapperSingleItem = WrapperMultiItems + " show-placeholder single-item"; // enables a placeholder when empty, and limits one entry


        internal static string RenderWithEditContext(DynamicEntity dynParent, IDynamicEntity subItem, string cbFieldName,  Guid? newGuid = null, IInPageEditingSystem edit = null)
        {
            if (edit == null)
                edit = new InPageEditingHelper(dynParent._Dependencies.BlockOrNull);

            var attribs = edit.ContextAttributes(dynParent, field: cbFieldName, newGuid: newGuid);
            var inner = subItem == null ? "": Render(dynParent._Dependencies.BlockOrNull, subItem.Entity).ToString();
            var cbClasses = edit.Enabled ? WrapperSingleItem : "";
            return string.Format(WrapperTemplate, new object[] { cbClasses, attribs, inner});
        }

        internal static string RenderListWithContext(DynamicEntity dynParent, string fieldName, string apps = null, int max = 100)
        {
            var innerBuilder = new StringBuilder();
            var found = dynParent.TryGetMember(fieldName, out var objFound);
            if (found && objFound is IList<DynamicEntity> items)
                foreach (var cb in items)
                    innerBuilder.Append(Render(cb._Dependencies.BlockOrNull, cb.Entity));

            // create edit object if missing...to re-use of the parent
            //if (edit == null)
            IInPageEditingSystem edit = new InPageEditingHelper(dynParent._Dependencies.BlockOrNull);

            return string.Format(WrapperTemplate, new object[]
            {
                edit.Enabled ? WrapperMultiItems : "",
                edit.ContextAttributes(dynParent, field: fieldName, apps: apps, max: max),
                innerBuilder
            });
        }
    }
}