using System;
using Newtonsoft.Json;
using ToSic.Eav;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web;
#if NET451
using HtmlString = System.Web.HtmlString;
#else
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif

namespace ToSic.Sxc.Edit.InPageEditingSystem
{
    public partial class InPageEditingHelper 
    {
        #region Context Attributes

        /// <inheritdoc/>
        public HtmlString ContextAttributes(IDynamicEntity target,
            string noParameterOrder = Eav.Parameters.Protector, 
            string field = null,
            string contentType = null, 
            Guid? newGuid = null,
            string apps = null,
            int max = 100)
        {
            Log.Add("ctx attribs - enabled:{Enabled}");
            if (!Enabled) return null;
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParameterOrder, nameof(ContextAttributes), $"{nameof(field)},{nameof(contentType)},{nameof(newGuid)}");

            if (field == null) throw new Exception("need parameter 'field'");

            var serialized = JsonConvert.SerializeObject(new 
            {
                apps,
                field,
                guid = newGuid.ToString(),
                max,
                parent = target.EntityId,
                parentGuid = target.EntityGuid,
                type = contentType ?? Settings.AttributeSetStaticNameContentBlockTypeName,
            });

            return new HtmlString(innerContentAttribute + "='" + serialized + "'");
        }

        /// <inheritdoc/>
        [PrivateApi]
        public HtmlString WrapInContext(object content,
            string noParameterOrder = Eav.Parameters.Protector,
            string tag = Constants.DefaultContextTag,
            bool full = false,
            bool? enableEdit = null,
            int instanceId = 0,
            int contentBlockId = 0
        )
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParameterOrder, nameof(WrapInContext), $"{nameof(tag)},{nameof(full)},{nameof(enableEdit)},{nameof(instanceId)},{nameof(contentBlockId)}");

            var renderingHelper = Block.Context.ServiceProvider.Build<IRenderingHelper>().Init(Block, Log);

            return new HtmlString(
               renderingHelper.WrapInContext(content.ToString(),
                    instanceId: instanceId > 0
                        ? instanceId
                        : Block?.ParentId ?? 0,
                    contentBlockId: contentBlockId > 0
                        ? contentBlockId
                        : Block?.ContentBlockId ?? 0,
                    editContext: enableEdit ?? Enabled)
            );
        }

        #endregion Context Attributes
    }
}