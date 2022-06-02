using System;
using System.Collections.Generic;
using System.Text;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.DI;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Blocks.Renderers
{
    internal class Simple
    {
        private static string EmptyMessage = "<!-- auto-render of item {0} -->";

        internal static string Render(IBlock parentBlock, IEntity entity, IGenerator<BlockFromEntity> blkFrmEntGen)
        {
            var log = new Log("Htm.Render", parentBlock.Log, "simple");

            // if not the expected content-type, just output a hidden html placeholder
            if (entity.Type.Name != AppConstants.ContentGroupRefTypeName)
            {
                log.A("empty, will return hidden html placeholder");
                return string.Format(EmptyMessage, entity.EntityId);
            }

            // render it
            log.A("found, will render");
            var cb = blkFrmEntGen.New.Init(parentBlock, entity, log);
            var result = cb.BlockBuilder.Run(false);

            // Special: during Run() various things are picked up like header changes, activations etc.
            // Depending on the code flow, it could have picked up changes of other templates (not this one)
            // because these were scoped, 
            // must attach additional info to the parent block, so it doesn't loose header changes and similar

            // Return resulting string
            return result.Html;
        }

        private const string WrapperTemplate = "<div class='{0}' {1}>{2}</div>";
        private const string WrapperMultiItems = "sc-content-block-list"; // tells quickE that it's an editable area
        private const string WrapperSingleItem = WrapperMultiItems + " show-placeholder single-item"; // enables a placeholder when empty, and limits one entry


        internal static string RenderWithEditContext(DynamicEntity parent, IDynamicEntity subItem, string cbFieldName,  Guid? newGuid, IEditService edit, IGenerator<BlockFromEntity> blkFrmEntGen)
        {
            var attribs = edit.ContextAttributes(parent, field: cbFieldName, newGuid: newGuid);
            var inner = subItem == null ? "": Render(parent._Dependencies.BlockOrNull, subItem.Entity, blkFrmEntGen);
            var cbClasses = edit.Enabled ? WrapperSingleItem : "";
            return string.Format(WrapperTemplate, new object[] { cbClasses, attribs, inner});
        }

        internal static string RenderListWithContext(DynamicEntity parent, string fieldName, string apps, int max, IEditService edit, IGenerator<BlockFromEntity> blkFrmEntGen)
        {
            var innerBuilder = new StringBuilder();
            var found = parent.TryGetMember(fieldName, out var objFound);
            if (found && objFound is IList<DynamicEntity> items)
                foreach (var cb in items)
                    innerBuilder.Append(Render(cb._Dependencies.BlockOrNull, cb.Entity, blkFrmEntGen));

            return string.Format(WrapperTemplate, new object[]
            {
                edit.Enabled ? WrapperMultiItems : "",
                edit.ContextAttributes(parent, field: fieldName, apps: apps, max: max),
                innerBuilder
            });
        }
    }
}