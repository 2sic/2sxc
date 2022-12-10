using System;
using System.Text.Json;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Lib.Logging;
using ToSic.Eav.Serialization;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.EditService
{
    public partial class EditService
    {
        #region Context Attributes

        /// <inheritdoc/>
        public IHybridHtmlString ContextAttributes(IDynamicEntity target,
            string noParamOrder = Parameters.Protector,
            string field = null,
            string contentType = null,
            Guid? newGuid = null,
            string apps = null,
            int max = 100)
        {
            Log.A("ctx attribs - enabled:{Enabled}");
            if (!Enabled) return null;
            Parameters.Protect(noParamOrder, $"{nameof(field)},{nameof(contentType)},{nameof(newGuid)}");

            if (field == null) throw new Exception("need parameter 'field'");

            var serialized = JsonSerializer.Serialize(new
            {
                apps,
                field,
                guid = newGuid.ToString(),
                max,
                parent = target.EntityId,
                parentGuid = target.EntityGuid,
                type = contentType ?? AppConstants.ContentGroupRefTypeName,
            }, JsonOptions.SafeJsonForHtmlAttributes);

            return new HybridHtmlString(innerContentAttribute + "='" + serialized + "'");
        }

        /// <inheritdoc/>
        [PrivateApi]
        public IHybridHtmlString WrapInContext(object content,
            string noParamOrder = Parameters.Protector,
            string tag = Constants.DefaultContextTag,
            bool full = false,
            bool? enableEdit = null,
            int instanceId = 0,
            int contentBlockId = 0
        )
        {
            Parameters.Protect(noParamOrder, $"{nameof(tag)},{nameof(full)},{nameof(enableEdit)},{nameof(instanceId)},{nameof(contentBlockId)}");

            var renderingHelper = _renderHelper.Value;

            return new HybridHtmlString(
               renderingHelper.WrapInContext(content.ToString(),
                    instanceId: instanceId > 0
                        ? instanceId
                        : Block.ParentId,
                    contentBlockId: contentBlockId > 0
                        ? contentBlockId
                        : Block.ContentBlockId,
                    editContext: enableEdit ?? Enabled)
            );
        }

        #endregion Context Attributes
    }
}
